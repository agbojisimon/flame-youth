namespace GlobalFlameMinistry.API.Exceptions
{
    public class EventFullException : Exception
    {
        public EventFullException(string message)
            : base(message)
        {
        }

        public EventFullException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}