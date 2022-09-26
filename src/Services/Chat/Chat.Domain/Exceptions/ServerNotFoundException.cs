namespace Chat.Domain.Exceptions;

public class ServerNotFoundException : NotFoundException
{
    public ServerNotFoundException(string id) : base($"Server '{id}' does not exist") { }
}
