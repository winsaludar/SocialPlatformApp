namespace Space.Domain.Exceptions;

public class InvalidSoulException : BadRequestException
{
    public InvalidSoulException(string name)
        : base($"Invalid soul '{name}'") { }
}
