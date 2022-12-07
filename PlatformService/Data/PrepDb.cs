using PlatformService.Models;

namespace PlatformService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app, bool isProd)
    {
        using(var serviceScope = app.ApplicationServices.CreateScope())
        {
            SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
        }
    }

    private static void SeedData(AppDbContext context, bool isProd)
    {
        if (isProd)
        {
            Console.WriteLine("--> Attempting to apply migration ...");
            try
            {
                // context.Database.Migrate();
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Could not run migration: {e.Message}");
            }
        }

        if (!context.Platforms.Any())
        {
            Console.WriteLine("---> Seeding data...");

            context.Platforms.AddRange(
                new Platform() {Name = "Dotnet", Publisher = "Tuanbq1", Cost = "10"},
                new Platform() {Name = "Java", Publisher = "Tuanbq3", Cost = "20"},
                new Platform() {Name = "Nodejs", Publisher = "Tuanbq2", Cost = "30"}
            );

            context.SaveChanges();
        }
        else 
        {
            Console.WriteLine("---> We already have data");
        }
    }
}