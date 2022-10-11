using Chat.Domain.SeedWork;
using Microsoft.AspNetCore.SignalR;

namespace Chat.API.Hubs;

public class ChatHub : Hub
{
    private readonly IRepositoryManager _repositoryManager;
    private Guid _serverId;

    public ChatHub(IRepositoryManager repositoryManager) => _repositoryManager = repositoryManager;

    public override async Task OnConnectedAsync()
    {
        HttpContext? context = Context.GetHttpContext();
        if (context == null)
            throw new HubException("HttpContext is null");

        // Make sure server id exist
        string? sid = context.GetRouteValue("serverId") as string;
        if (string.IsNullOrEmpty(sid) || !Guid.TryParse(sid, out _serverId))
            throw new HubException($"Server '{sid}' not found");
        var server = await _repositoryManager.ServerRepository.GetByIdAsync(_serverId);
        if (server is null)
            throw new HubException($"Server '{sid}' not found");

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
