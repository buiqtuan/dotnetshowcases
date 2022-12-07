using CommandService.Models;

namespace CommandService.Data;

public class CommandRepo : ICommandRepo
{
    private readonly AppDbContext _context;

    public CommandRepo(AppDbContext context)
    {
        _context = context;
    }

    public bool SaveChange()
    {
        return (_context.SaveChanges() >= 0);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _context.Platforms.ToList();
    }

    public void CreatePlatform(Platform platform)
    {
        if (platform is null)
        {
            throw new ArgumentNullException();
        }

        _context.Platforms.Add(platform!);
    }

    public bool PlatformExits(int platformId)
    {
        return _context.Platforms.Any(p => p.Id == platformId);
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
        return _context.Commands
            .Where(c => c.PlatformId == platformId)
            .OrderBy(c => c.Platform!.Name);
    }

    public Command GetCommand(int platformId, int commandId)
    {
        return _context.Commands
            .Where(c => c.PlatformId == platformId && c.Id == commandId).FirstOrDefault()!;
    }

    public void CreateCommand(int platformId, Command command)
    {
        if (command is null)
        {
            throw new ArgumentNullException();
        }

        command.PlatformId = platformId;
        _context.Commands.Add(command);
    }

    public bool ExternalPlatformExist(int externalPlatformId)
    {
        return _context.Platforms.Any(p => p.Id == externalPlatformId);
    }
}