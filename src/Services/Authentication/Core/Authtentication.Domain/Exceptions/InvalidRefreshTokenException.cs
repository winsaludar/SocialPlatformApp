namespace Authentication.Domain.Exceptions;

public class InvalidRefreshTokenException : BadRequestException
{
    public InvalidRefreshTokenException() :
        base("Invalid refresh token")
    { }
}
