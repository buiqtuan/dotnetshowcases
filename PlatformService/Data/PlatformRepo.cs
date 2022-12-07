using PlatformService.Models;
using System.Linq;

namespace PlatformService.Data;

public class PlatformRepo : IPlatformRepo
{
    private readonly AppDbContext _context;

    public PlatformRepo(AppDbContext context) => _context = context;

    public void CreatePlatform(Platform platform)
    {
        if (platform is null)
        {
            throw new ArgumentException(nameof(platform));
        }

        _context.Add(platform);
    }

    public IEnumerable<Platform> GetAllPlatform()
    {
        return _context.Platforms.ToList();
    }

    public Platform GetPlatFormById(int id)
    {
        return _context.Platforms.FirstOrDefault(p => p.Id == id);
    }

    public bool SaveChange()
    {
        return (_context.SaveChanges() >= 0);
    }
}