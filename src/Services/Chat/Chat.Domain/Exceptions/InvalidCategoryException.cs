namespace Chat.Domain.Exceptions;

public class InvalidCategoryException : BadRequestException
{
    public InvalidCategoryException(string name, int id) : base($"Invalid category name: '{name}' with ID of: '{id}'") { }
}
