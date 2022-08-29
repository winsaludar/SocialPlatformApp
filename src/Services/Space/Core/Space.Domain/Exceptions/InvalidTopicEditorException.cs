namespace Space.Domain.Exceptions;

public class InvalidTopicEditorException : BadRequestException
{
    public InvalidTopicEditorException(string name)
    : base($"Invalid author '{name}'") { }
}
