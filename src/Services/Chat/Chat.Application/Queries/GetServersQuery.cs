using Chat.Application.DTOs;
using MediatR;

namespace Chat.Application.Queries;

public record GetServersQuery : IRequest<IEnumerable<ServerDto>>
{
}
