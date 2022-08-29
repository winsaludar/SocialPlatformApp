namespace Space.Domain.Exceptions;

public class InvalidTopicIdException : BadRequestException
{
    public InvalidTopicIdException(Guid id)
    : base($"Invalid topic id '{id}'") { }
}
