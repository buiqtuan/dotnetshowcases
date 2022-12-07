using CommandService.SyncDataServices.Grpc;
using CommandService.Models;

namespace CommandService.Data;

public static class PrepDb
{

    public static void Population(IApplicationBuilder builder)
    {
        using(var scope = builder.ApplicationServices.CreateScope()) {
            var grpcClient = scope.ServiceProvider.GetService<IPlatformDataClient>();

            var platforms = grpcClient!.ReturnAllPlatforms();

            SeedData(scope.ServiceProvider.GetService<ICommandRepo>()! ,platforms);
        }
    }

    private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
    {
        Console.WriteLine("--> Seeding new platforms...");

        foreach(var plat in platforms)
        {
            if (!repo.ExternalPlatformExist(plat.ExternalId))
            {
                Console.WriteLine($"--> Add platform {plat.ExternalId} to DB");
                repo.CreatePlatform(plat);
            }
        }

        repo.SaveChange();
    }
}