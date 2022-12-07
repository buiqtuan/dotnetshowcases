using CommandService.Models;

namespace CommandService.Data;

public interface ICommandRepo
{
    bool SaveChange();

    IEnumerable<Platform> GetAllPlatforms();

    void CreatePlatform(Platform platform);

    bool PlatformExits(int platformId);

    bool ExternalPlatformExist(int externalPlatformId);

    IEnumerable<Command> GetCommandsForPlatform(int platformId);

    Command GetCommand(int platformId, int commandId);

    void CreateCommand(int platformId, Command command);
}