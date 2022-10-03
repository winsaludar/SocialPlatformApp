namespace Chat.Domain.Exceptions;

public class ChannelNameAlreadyExistException : BadRequestException
{
    public ChannelNameAlreadyExistException(string name) : base($"Channel '{name}' is already exist") { }
}
