using AutoMapper;
using System.Text.Json;
using CommandService.Dtos;
using CommandService.Data;
using CommandService.Models;

namespace CommandService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly IMapper _mapper;

    public EventProcessor
    (
        IServiceScopeFactory scopeFactory,
        IMapper mapper
    )
    {
        _mapper = mapper;
        _scopeFactory = scopeFactory;
    }

    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch(eventType)
        {
            case EventType.PLATFORM_PUBLISH:
                //TODO
                break;
            default:
                break;
        }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("--> Determining Event ");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        switch(eventType!.Event)
        {
            case "Platform_Publish":
                Console.WriteLine("--> Platform publish detected...");
                return EventType.PLATFORM_PUBLISH;
            default:
                Console.WriteLine("--> Could not determined event type...");
                return EventType.UNDERTERMINED;
        }
    }

    private void AddPlatform(string platformPublishMessage)
    {
        using(var scope = _scopeFactory.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

            var platformPublishedDtop = JsonSerializer.Deserialize<PlatformPublishDto>(platformPublishMessage);

            try
            {
                var platform = _mapper.Map<Platform>(platformPublishMessage);

                if (!repo.ExternalPlatformExist(platform.ExternalId))
                {
                    repo.CreatePlatform(platform);
                    repo.SaveChange();
                    Console.WriteLine($"--> Platform added: {platform.ToString()}");
                }
                else
                {
                    Console.WriteLine($"--> Platform has already existed {platform.ExternalId}");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"--> Could not add platform to DB {e.Message}");
            }
        }
    }
}

public enum EventType
{
    PLATFORM_PUBLISH,
    UNDERTERMINED
}