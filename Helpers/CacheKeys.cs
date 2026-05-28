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
    public const string SermonPublished = "sermons:published_p{0}_s{1}_f{2}";
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
    public const string EventsUpcoming = "events:upcoming_p{0}_s{1}";
    public const string EventId = "events:id_{0}";
    public const string EventSlug = "events:slug_{0}";

    // ── ANNOUNCEMENTS ──────────────────────────────────────────────────────────
    public const string AnnouncementsPublished = "announcements:published";
    public const string AnnouncementId = "announcements:id_{0}";
    public const string AnnouncementSlug = "announcements:slug_{0}";

    // ── BLOG POSTS ─────────────────────────────────────────────────────────────
    public const string BlogPublished = "blog:published_p{0}_s{1}";
    public const string BlogId = "blog:id_{0}";
    public const string BlogSlug = "blog:slug_{0}";

    // ── TESTIMONIES ────────────────────────────────────────────────────────────
    public const string TestimoniesApproved = "testimonies:approved_p{0}_s{1}";
}
