namespace Chat.Domain.Exceptions;

public class UsernameAlreadyExistException : BadRequestException
{
    public UsernameAlreadyExistException(string username) : base($"Username '{username}' is already exist") { }
}
