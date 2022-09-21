using MediatR;

namespace Chat.Events.Commands;

public class CreateServerCommand : IRequest<string>
{
}
