namespace Chat.Domain.Exceptions;

public class UserIsNotAMemberException : BadRequestException
{
    public UserIsNotAMemberException(string usernameOrId) : base($"User '{usernameOrId}' is not a member") { }
}
