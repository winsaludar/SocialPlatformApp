namespace Authentication.Domain.Exceptions;

public sealed class InvalidEmailException : BadRequestException
{
    public InvalidEmailException(string email)
        : base($"Invalid email '{email}'") { }
}
