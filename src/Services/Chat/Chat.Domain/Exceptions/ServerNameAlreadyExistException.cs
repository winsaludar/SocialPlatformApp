namespace Chat.Domain.Exceptions;

public class ServerNameAlreadyExistException : BadRequestException
{
    public ServerNameAlreadyExistException(string name) : base($"Name '{name}' is already exist") { }
}
