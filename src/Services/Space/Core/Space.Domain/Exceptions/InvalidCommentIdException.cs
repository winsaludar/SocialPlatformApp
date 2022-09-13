namespace Space.Domain.Exceptions;

public class InvalidCommentIdException : BadRequestException
{
    public InvalidCommentIdException(Guid id)
        : base($"Invalid comment id '{id}'") { }
}