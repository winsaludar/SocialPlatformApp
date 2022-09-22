namespace Authentication.Core.Exceptions;

public class InvalidRefreshTokenException : BadRequestException
{
    public InvalidRefreshTokenException() : base("Invalid refresh token") { }
}

