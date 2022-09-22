namespace Chat.Domain.Exceptions;

public class NameAlreadyInUseException : BadRequestException
{
    public NameAlreadyInUseException(string name)
        : base($"Name '{name}' is already in use") { }
}
