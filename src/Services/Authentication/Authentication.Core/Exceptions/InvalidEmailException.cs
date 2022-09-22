namespace Authentication.Core.Exceptions;

public class InvalidEmailException : BadRequestException
{
    public InvalidEmailException(string email) : base($"Invalid email '{email}'") { }
}

