using System.Globalization;

namespace Shared.Exceptions;

public class BusinessLogicException : Exception
{
    public BusinessLogicException() : base() {}

    public BusinessLogicException(string message) : base(message) { }

    public BusinessLogicException(string message, params object[] args) : base(string.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
}