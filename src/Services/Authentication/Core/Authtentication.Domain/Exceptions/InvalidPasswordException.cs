namespace Authentication.Domain.Exceptions;

public class InvalidPasswordException : BadRequestException
{
    public InvalidPasswordException() : base("Password does not meet the required criteria") { }
}
