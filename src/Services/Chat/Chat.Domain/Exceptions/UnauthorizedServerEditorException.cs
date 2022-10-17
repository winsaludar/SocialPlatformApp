namespace Chat.Domain.Exceptions;

public class UnauthorizedServerEditorException : UnauthorizedAccessException
{
    public UnauthorizedServerEditorException(string emailOrId) : base($"User '{emailOrId}' is unauthorized to edit the server") { }
}
