using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;
using PlatformService.AsyncDataService;
using PlatformService.SyncDataServices.Grpc;
using System.Reflection;

public class Startup
{
    private readonly IConfiguration Configuration;

    private readonly IWebHostEnvironment _evn;
    
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        _evn = env;
    }

    public void ConfigurationServices(IServiceCollection services) 
    {
        if (_evn.IsProduction())
        {
            Console.WriteLine("--> Using Azure SQL DB ...");

            services.AddDbContext<AppDbContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("PlatformsConn"), x => 
                x.MigrationsAssembly(typeof(Startup).GetType().Assembly.GetName().Name)));
        }
        else
        {
            Console.WriteLine("--> Using InMemDB ...");
            services.AddDbContext<AppDbContext>(options => 
                options.UseInMemoryDatabase("InMem"));
        }

        services.AddControllers();
        services.AddScoped<IPlatformRepo, PlatformRepo>();
        services.AddSingleton<IMessageBusClient, MessageBusClient>();
        services.AddGrpc();
        services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddSwaggerGen(c => 
        {
            c.SwaggerDoc("v1", new OpenApiInfo 
            {
                Title = "PlatformService",
                Version = "1.0"
            });

            var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
        });

        Console.WriteLine($"--> CommandService Endpoint: {Configuration["CommandService"]}");
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) 
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService V1"));
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseEndpoints(endpoint => 
        {
            endpoint.MapControllers();
            endpoint.MapGrpcService<GrpcPlatformService>();

            endpoint.MapGet("/protos/platforms.proto", async context => 
            {
                await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
            });
        });

        PrepDb.PrepPopulation(app, _evn.IsProduction());
    }
}