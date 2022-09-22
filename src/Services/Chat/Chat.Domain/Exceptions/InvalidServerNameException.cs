namespace Chat.Domain.Exceptions;

public class InvalidServerNameException : BadRequestException
{
    public InvalidServerNameException(string name)
        : base($"Invalid server name '{name}'") { }
}
