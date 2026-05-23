namespace SmartOfferBooking.Application.Exceptions;

public class AppException : Exception
{
    public int StatusCode { get; }

    public AppException(string message, int statusCode = 400) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, 404) { }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Unauthorized") : base(message, 401) { }
}

public class ConflictException : AppException
{
    public ConflictException(string message) : base(message, 409) { }
}

public class ValidationException : AppException
{
    public IEnumerable<string> ValidationErrors { get; }

    public ValidationException(string message, IEnumerable<string>? errors = null)
        : base(message, 400)
    {
        ValidationErrors = errors ?? Array.Empty<string>();
    }
}
