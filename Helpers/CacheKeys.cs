namespace GlobalFlameMinistry.API.Helpers;

public static class CacheKeys
{
    // ── TAGS ───────────────────────────────────────────────────────────────────
    public const string TagSermons = "tag:sermons";
    public const string TagBooks = "tag:books";
    public const string TagMinistries = "tag:ministries";
    public const string TagEvents = "tag:events";
    public const string TagAnnouncements = "tag:announcements";
    public const string TagBlog = "tag:blog";
    public const string TagTestimonies = "tag:testimonies";

    // ── SERMONS ────────────────────────────────────────────────────────────────
    public const string SermonPublished = "sermons:published_p{0}_s{1}_f{2}_c{3}";
    public const string SermonId = "sermons:id_{0}";
    public const string SermonSlug = "sermons:slug_{0}";

    // ── BOOKS ──────────────────────────────────────────────────────────────────
    public const string BooksPublished = "books:published_p{0}_s{1}";
    public const string BookId = "books:id_{0}";
    public const string BookSlug = "books:slug_{0}";

    // ── MINISTRIES ─────────────────────────────────────────────────────────────
    public const string MinistriesAll = "ministries:all";
    public const string MinistryId = "ministries:id_{0}";
    public const string MinistrySlug = "ministries:slug_{0}";

    // ── EVENTS ─────────────────────────────────────────────────────────────────
    // v2: EventResponseDto gained ConferenceTheme / AnchorScripture /
    // AnchorScriptureReference. The version segment ensures any DTO cached
    // before this change (which lacks the new fields) is not served.
    public const string EventsUpcoming = "events:v2:upcoming_p{0}_s{1}";
    public const string EventId = "events:v2:id_{0}";
    public const string EventSlug = "events:v2:slug_{0}";

    // ── ANNOUNCEMENTS ──────────────────────────────────────────────────────────
    public const string AnnouncementsPublished = "announcements:published";
    public const string AnnouncementId = "announcements:id_{0}";
    public const string AnnouncementSlug = "announcements:slug_{0}";

    // ── BLOG POSTS ─────────────────────────────────────────────────────────────
    // v4: BlogPostResponseDto gained EventStartsAt / EventEndsAt.
    // any DTO cached before this change (which lacks the header video) is not served.
    // {0}=page {1}=size {2}=department {3}=search {4}=isPublished {5}=featuredOnHero
    public const string BlogPublished = "blog:v4:list_p{0}_s{1}_d{2}_q{3}_pub{4}_feat{5}";
    public const string BlogId = "blog:v4:id_{0}";
    public const string BlogSlug = "blog:v4:slug_{0}";
    public const string BlogDepartments = "blog:departments";
    public const string BlogRelated = "blog:v4:related_{0}_t{1}";

    // ── TESTIMONIES ────────────────────────────────────────────────────────────
    public const string TestimoniesApproved = "testimonies:approved_p{0}_s{1}";

    // ── HERO SLIDES ────────────────────────────────────────────────────────────
    // v3: HeroSlideDto gained Slug / HasWriteUp / ContentBlocks (clip write-ups).
    // The version segment ensures any DTO cached before this change (which lacks
    // the new fields) is not served.
    public const string TagHeroSlides = "tag:hero-slides";
    public const string HeroSlidesActive = "hero-slides:v3:active";
    public const string HeroClips = "hero-slides:v3:clips";
    public const string HeroClipsFeatured = "hero-slides:v3:clips:featured";
    public const string HeroClipSlug = "hero-slides:v3:clip_{0}";
}
