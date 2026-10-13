namespace GlobalFlameMinistry.API.Exceptions
{
    public class EventRegistrationConflictException : Exception
    {
        public EventRegistrationConflictException(string message)
            : base(message)
        {
        }

        public EventRegistrationConflictException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}