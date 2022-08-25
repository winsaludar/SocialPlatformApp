namespace Space.Domain.Exceptions
{
    public class SoulNotMemberException : BadRequestException
    {
        public SoulNotMemberException(string soulName, string spaceName)
            : base($"Soul '{soulName}' is not a member of the '{spaceName}' space") { }
    }
}
