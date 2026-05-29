namespace VisitBookingSystem.Exceptions
{
    public abstract class BaseException : Exception
    {
        public abstract int StatusCode { get; }
        protected BaseException(string message) : base(message) { }
    }

    public class NotFoundException : BaseException
    {
        public override int StatusCode => 404;
        public NotFoundException(string message) : base(message) { }
    }

    public class BusinessRuleException : BaseException
    {
        public override int StatusCode => 400;
        public BusinessRuleException(string message) : base(message) { }
    }
}