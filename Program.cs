using System.Net.Http.Headers;
using System.Text;
using GlobalFlameMinistry.API.Configuration;
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
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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
    options.AddPolicy("DevCors", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        // In production replace AllowAnyOrigin() with your actual frontend URL
        // Example: .WithOrigins("https://globalflameministry.com")
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
        // Zero means tokens expire exactly on time — no grace period
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

//APPLICATION SERVICES
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
builder.Services.AddHttpClient<IDonationService, DonationService>();
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
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("DevCors");

app.UseAuthentication();
// Who are you?
app.UseAuthorization();
// What are you allowed to do?
app.MapControllers();

app.Run();