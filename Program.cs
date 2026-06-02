using System.Net.Http.Headers;
using System.Text;
using System.Threading.RateLimiting;
using GlobalFlameMinistry.API.Configuration;
using Microsoft.AspNetCore.RateLimiting;
using GlobalFlameMinistry.API.Data;
using GlobalFlameMinistry.API.Fillters;
using GlobalFlameMinistry.API.Filters;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Interfaces.Account;
using GlobalFlameMinistry.API.Interfaces.Admin;
using GlobalFlameMinistry.API.Interfaces.Auth;
using GlobalFlameMinistry.API.Interfaces.BulkEmail;
using GlobalFlameMinistry.API.Interfaces.Counselling;
using GlobalFlameMinistry.API.Interfaces.Email;
using GlobalFlameMinistry.API.Interfaces.Ministry;
using GlobalFlameMinistry.API.Models;
using GlobalFlameMinistry.API.Repositories;
using GlobalFlameMinistry.API.Repository;
using GlobalFlameMinistry.API.Services;
using GlobalFlameMinistry.API.Services.Admin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Required for Npgsql to handle DateTime correctly
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// Heroku dynamically assigns a port — we must listen on it
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// CONTROLLER + FILTER + JSON CONFIGURATION
builder.Services.AddControllers(options =>
{
    // Global filters apply to every controller automatically
    options.Filters.Add<ApiResponseFilter>();
    options.Filters.Add<GlobalExceptionFilter>();
    options.Filters.Add<ValidationFilter>();
})
.AddNewtonsoftJson(options =>
{
    // Prevents circular reference errors when EF Core navigations are serialized
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

// SWAGGER CONFIGURATION
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Global Flame Ministry API",
        Version = "v1"
    });

    // Adds the Authorize button in Swagger so you can test JWT-protected routes
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter your JWT token here. Example: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS POLICY
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionCors", policy =>
    {
        policy
            .WithOrigins(
                "https://globalflameministry.org",
                "https://www.globalflameministry.org",
                "http://localhost:5173" // keep for local development
            )
            .AllowCredentials()
            .WithHeaders("Authorization", "Content-Type")
            .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE");
    });
});

// DATABASE CONTEXT
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// IDENTITY CONFIGURATION
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    // Email must be confirmed before login is allowed
    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedAccount = true;
    options.User.RequireUniqueEmail = true;

    // Account lockout — 5 failed attempts = 15 minute lockout
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;

    // Password rules
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

//JWT AUTHENTICATION
builder.Services.AddAuthentication(options =>
{
    // Set JWT Bearer as the default scheme for everything
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]!)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

});

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(1); // Match what email says
});

builder.Services.AddHttpClient("BrevoClient", client =>
{
    client.BaseAddress = new Uri("https://api.brevo.com/v3/");
    client.DefaultRequestHeaders.Accept
        .Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

//EMAIL SETTINGS AND DEPENDENCY INJECTION
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailSender, EmailSender>();

// REDIS + HYBRID CACHE (two-level: L1 = in-memory, L2 = Redis)
// HybridCache uses IMemoryCache internally as L1, no need for separate AddMemoryCache.
//
// Heroku deployment: provision "Heroku Data for Redis" addon, then REDIS_URL
// environment variable is auto-set. The code below parses the redis:// URL format.
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
var redisUrl = Environment.GetEnvironmentVariable("REDIS_URL");
if (!string.IsNullOrEmpty(redisUrl))
{
    try
    {
        var uri = new Uri(redisUrl);
        var password = uri.UserInfo?.Split(':')?.Length > 1 ? uri.UserInfo.Split(':')[1] : "";
        redisConnectionString = $"{uri.Host}:{uri.Port},password={password},ssl=True,abortConnect=False";
    }
    catch
    {
        // Fallback to config if URL parsing fails
    }
}

// Ensure abortConnect is set so Redis unavailability doesn't crash the app
if (!redisConnectionString.Contains("abortConnect", StringComparison.OrdinalIgnoreCase))
{
    redisConnectionString += ",abortConnect=false";
}

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "GlobalFlame_";
});

builder.Services.AddHybridCache(options =>
{
    options.MaximumPayloadBytes = 1024 * 1024 * 5;
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(10),
        LocalCacheExpiration = TimeSpan.FromMinutes(2)
    };
});

// RATE LIMITING
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Global catch-all: 100 requests per minute per IP
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    // Login: 10 requests per minute
    options.AddFixedWindowLimiter("LoginPolicy", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    // Registration: 5 requests per minute
    options.AddFixedWindowLimiter("RegistrationPolicy", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    // Password reset / resend: 3 requests per 15 minutes
    options.AddFixedWindowLimiter("ForgotPasswordPolicy", opt =>
    {
        opt.PermitLimit = 3;
        opt.Window = TimeSpan.FromMinutes(15);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    // Bulk email: 5 requests per hour
    options.AddFixedWindowLimiter("BulkEmailPolicy", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromHours(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    // General policy for public write endpoints: 20 requests per minute
    options.AddFixedWindowLimiter("GeneralPolicy", opt =>
    {
        opt.PermitLimit = 20;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    // Auth catch-all (refresh, confirm email, reset-password): 10 requests per minute
    options.AddFixedWindowLimiter("AuthCatchAllPolicy", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
});

//APPLICATION SERVICES
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IPrayerRequestRepository, PrayerRequestRepository>();
builder.Services.AddScoped<IPrayerRequestService, PrayerRequestService>();
builder.Services.AddScoped<ITestimonyRepository, TestimonyRepository>();
builder.Services.AddScoped<ITestimonyService, TestimonyService>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();
builder.Services.AddScoped<ISermonRepository, SermonRepository>();
builder.Services.AddScoped<ISermonService, SermonService>();
builder.Services.AddScoped<IEventRegistrationRepository, EventRegistrationRepository>();
builder.Services.AddScoped<IEventRegistrationService, EventRegistrationService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddHttpClient("DonationClient");
builder.Services.AddScoped<IDonationService, DonationService>();
builder.Services.AddScoped<IDonationRepository, DonationRepository>();
builder.Services.AddScoped<IAdminDonationService, AdminDonationService>();
builder.Services.Configure<BrevoSettings>(builder.Configuration.GetSection("Brevo"));
builder.Services.AddScoped<IBulkEmailRepository, BulkEmailRepository>();
builder.Services.AddScoped<IBulkEmailService, BulkEmailService>();
builder.Services.AddHostedService<EmailSchedulerService>();
builder.Services.AddScoped<IMinistryRepository, MinistryRepository>();
builder.Services.AddScoped<IMinistryService, MinistryService>();
builder.Services.AddScoped<ICounsellingRepository, CounsellingRepository>();
builder.Services.AddScoped<ICounsellingService, CounsellingService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<IBlogPostService, BlogPostService>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();


        // Auto-apply any pending migrations on startup
        await context.Database.MigrateAsync();
        await DataSeeder.SeedAdminAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database");
    }
}

// MIDDLEWARE PIPELINE
// ORDER MATTERS — don't rearrange these
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();

app.UseHttpsRedirection();

// SECURITY HEADERS
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
    if (!context.Request.Host.Value.Contains("localhost"))
    {
        context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
    }
    await next();
});

app.UseCors("ProductionCors");

app.UseAuthentication();
// Who are you?
app.UseAuthorization();
// What are you allowed to do?
app.MapControllers();

app.Run();