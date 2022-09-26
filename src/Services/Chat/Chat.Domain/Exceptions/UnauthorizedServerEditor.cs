namespace Chat.Domain.Exceptions;

public class UnauthorizedServerEditor : UnauthorizedAccessException
{
    public UnauthorizedServerEditor(string email) : base($"Email '{email}' is unauthorized to edit the server") { }
}
