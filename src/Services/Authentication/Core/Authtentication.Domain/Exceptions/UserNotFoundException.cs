namespace Authentication.Domain.Exceptions;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(string email)
        : base($"The email '{email}' is not exist in the database.") { }
}
