using MediatR;

namespace Chat.Application.Commands;

public class CreateServerCommandHandler : IRequestHandler<CreateServerCommand, string>
{
    public async Task<string> Handle(CreateServerCommand request, CancellationToken cancellationToken)
    {
        return await Task.Run(() => "Server created!");
    }
}

