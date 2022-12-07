using PlatformService.Models;

namespace PlatformService.Data;

public interface IPlatformRepo
{
    bool SaveChange();

    IEnumerable<Platform> GetAllPlatform();

    Platform GetPlatFormById(int id);

    void CreatePlatform(Platform platform);
}