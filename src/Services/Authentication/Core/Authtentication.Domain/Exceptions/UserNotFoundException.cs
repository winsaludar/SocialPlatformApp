namespace Authentication.Domain.Exceptions;

public sealed class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(string email)
        : base($"The email '{email}' is not exist in the database.") { }
}
