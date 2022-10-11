using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using Microsoft.AspNetCore.SignalR;

namespace Chat.API.Hubs;

public class ChatHub : Hub
{
    private readonly IRepositoryManager _repositoryManager;

    public ChatHub(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public override async Task OnConnectedAsync()
    {
        // Make sure serverId is valid
        await GetServerAsync();

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    [HubMethodName("joinChannel")]
    public async Task JoinChannelAsync(Guid channelId)
    {
        Channel channel = await GetChannelAsync(channelId);
        await Groups.AddToGroupAsync(Context.ConnectionId, channel.Name);
    }

    [HubMethodName("sendMessage")]
    public async Task SendMessageAsync(Guid channelId, string username, string message)
    {
        Channel channel = await GetChannelAsync(channelId);
        DateTime dateSent = DateTime.UtcNow;
        await Clients.Groups(channel.Name).SendAsync("broadcastMessage", new { username, message, dateSentUtc = dateSent });
    }

    [HubMethodName("leaveChannel")]
    public async Task LeaveChannelAsync(Guid channelId)
    {
        Channel channel = await GetChannelAsync(channelId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, channel.Name);
    }

    private async Task<Server> GetServerAsync()
    {
        string? sid = Context.GetHttpContext()?.GetRouteValue("serverId") as string;
        if (string.IsNullOrEmpty(sid) || !Guid.TryParse(sid, out Guid serverId))
            throw new HubException($"Invalid server id: '{sid}'");
        Server? server = await _repositoryManager.ServerRepository.GetByIdAsync(serverId);
        if (server is null)
            throw new HubException($"Server '{sid}' not found");

        return server;
    }

    private async Task<Channel> GetChannelAsync(Guid channelId)
    {
        Server server = await GetServerAsync();
        Channel? channel = server.Channels.FirstOrDefault(x => x.Id == channelId);
        if (channel is null)
            throw new HubException($"Channel '{channelId}' not found");

        return channel;
    }
}
