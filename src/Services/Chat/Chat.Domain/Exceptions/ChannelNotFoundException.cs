namespace Chat.Domain.Exceptions;

public class ChannelNotFoundException : NotFoundException
{
    public ChannelNotFoundException(string id) : base($"Channel '{id}' does not exist") { }
}
