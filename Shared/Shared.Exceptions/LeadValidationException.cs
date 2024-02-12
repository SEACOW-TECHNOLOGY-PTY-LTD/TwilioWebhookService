namespace Shared.Exceptions
{
    public class LeadValidationException : Exception
    {
        public LeadValidationException()
        {
        }

        public LeadValidationException(string message)
            : base(message)
        {
        }

        public LeadValidationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}