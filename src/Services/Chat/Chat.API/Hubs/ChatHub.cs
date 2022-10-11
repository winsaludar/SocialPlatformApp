using Chat.API.Extensions;
using Chat.Application.Queries;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Aggregates.UserAggregate;
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
        Server? server = await _mediator.Send(query);
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
        Channel channel = await GetChannel(channelId);
        await Groups.AddToGroupAsync(Context.ConnectionId, channel.Name);
    }

    [HubMethodName("sendMessage")]
    public async Task SendMessageAsync(Guid channelId, string message)
    {
        Channel channel = await GetChannel(channelId);
        User user = await GetUser();

        await Clients.Groups(channel.Name).SendAsync("broadcastMessage", new { username = user.Username, message, dateSentUtc = DateTime.UtcNow });
    }

    [HubMethodName("leaveChannel")]
    public async Task LeaveChannelAsync(Guid channelId)
    {
        Channel channel = await GetChannel(channelId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, channel.Name);
    }

    private async Task<Channel> GetChannel(Guid channelId)
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

    private async Task<User> GetUser()
    {
        if (!Context.User.IsValid())
            throw new HubException("User is invalid");

        string email = Context.User!.Identity!.Name!;
        GetUserByEmailQuery query = new(email);
        var user = await _mediator.Send(query);
        if (user is null)
            throw new HubException($"User '{email}' does not exist");

        return user;
    }
}
