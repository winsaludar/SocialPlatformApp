namespace Authentication.Domain.Exceptions
{
    public sealed class UserAlreadyExistException : BadRequestException
    {
        public UserAlreadyExistException(string email)
            : base($"The email '{email}' is already exist in the database.") { }
    }
}
