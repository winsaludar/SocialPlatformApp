namespace Chat.Domain.Exceptions;

public class UserIsAlreadyAMemberException : BadRequestException
{
    public UserIsAlreadyAMemberException(string usernameOrId) : base($"User '{usernameOrId}' is already a member") { }
}
