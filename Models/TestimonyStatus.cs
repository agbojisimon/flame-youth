namespace GlobalFlameMinistry.API.Models
{
  public enum TestimonyStatus
  {
    Pending = 0,    // Just submitted, awaiting admin review
    Approved = 1,   // Admin approved — visible publicly
    Rejected = 2    // Admin rejected — not visible publicly
  }
}