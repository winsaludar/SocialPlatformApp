using MediatR;

namespace Chat.Events.Commands;

public class CreateServerCommandHandler : IRequestHandler<CreateServerCommand, string>
{
    public async Task<string> Handle(CreateServerCommand request, CancellationToken cancellationToken)
    {
        return await Task.Run(() => "Server created");
    }
}
