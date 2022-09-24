namespace Chat.Domain.Exceptions;

public class EmailAlreadyExistException : BadRequestException
{
    public EmailAlreadyExistException(string email) : base($"Email '{email}' is already exist") { }
}
