namespace Space.Domain.Exceptions
{
    public class SoulMemberAlreadyException : BadRequestException
    {
        public SoulMemberAlreadyException(string soulName, string spaceName)
            : base($"Soul '{soulName}' is already a member of the '{spaceName}' space") { }
    }
}
