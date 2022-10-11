using Chat.Application.DTOs;
using Chat.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chat.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMediator _mediator;

    public ChatHub(IMediator mediator) => _mediator = mediator;

    public override async Task OnConnectedAsync()
    {
        string? sid = Context.GetHttpContext()?.GetRouteValue("serverId") as string;
        if (string.IsNullOrEmpty(sid) || !Guid.TryParse(sid, out Guid serverId))
            throw new HubException($"Invalid server id: '{sid}'");

        GetServerQuery query = new(serverId);
        var server = await _mediator.Send(query);
        if (server is null)
            throw new HubException($"Server '{serverId}' not found");

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    [HubMethodName("joinChannel")]
    public async Task JoinChannelAsync(Guid channelId)
    {
        ChannelDto channel = await GetChannel(channelId);
        await Groups.AddToGroupAsync(Context.ConnectionId, channel.Name);
    }

    [HubMethodName("sendMessage")]
    public async Task SendMessageAsync(Guid channelId, string username, string message)
    {
        ChannelDto channel = await GetChannel(channelId);
        DateTime dateSent = DateTime.UtcNow;
        await Clients.Groups(channel.Name).SendAsync("broadcastMessage", new { username, message, dateSentUtc = dateSent });
    }

    [HubMethodName("leaveChannel")]
    public async Task LeaveChannelAsync(Guid channelId)
    {
        ChannelDto channel = await GetChannel(channelId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, channel.Name);
    }

    public async Task<ChannelDto> GetChannel(Guid channelId)
    {
        string? sid = Context.GetHttpContext()?.GetRouteValue("serverId") as string;
        if (string.IsNullOrEmpty(sid) || !Guid.TryParse(sid, out Guid serverId))
            throw new HubException($"Invalid server id: '{sid}'");

        GetChannelQuery query = new(serverId, channelId);
        var channel = await _mediator.Send(query);
        if (channel is null)
            throw new HubException($"Channel '{channelId}' not found");

        return channel;
    }
}
