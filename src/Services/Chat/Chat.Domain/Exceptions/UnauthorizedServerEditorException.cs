namespace Chat.Domain.Exceptions;

public class UnauthorizedServerEditorException : UnauthorizedAccessException
{
    public UnauthorizedServerEditorException(string email) : base($"Email '{email}' is unauthorized to edit the server") { }
}
