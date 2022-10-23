namespace Space.Services.Abstraction;

public interface IServiceManager
{
    ISpaceService SpaceService { get; }
    ISoulService SoulService { get; }
}
