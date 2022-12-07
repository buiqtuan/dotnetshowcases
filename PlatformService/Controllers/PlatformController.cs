using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Models;
using AutoMapper;
using PlatformService.Dtos;
using PlatformService.SyncDataServices.Http;
using PlatformService.AsyncDataService;

namespace PlatformService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatformsController : ControllerBase
{
    private readonly ICommandDataClient _commandDataClient;

    private readonly IPlatformRepo _iPlatformRepo;

    private readonly IMapper _iMapper;

    private readonly IMessageBusClient _messageBusClient;
    public PlatformsController
    (
        IPlatformRepo repo, 
        IMapper mapper,
        ICommandDataClient commandDataClient,
        IMessageBusClient messageBusClient
    )
    {
        _iMapper = mapper;
        _iPlatformRepo = repo;
        _commandDataClient = commandDataClient;
        _messageBusClient = messageBusClient;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        Console.WriteLine("--> Getting platforms.....");

        var platformItem = _iPlatformRepo.GetAllPlatform();

        return Ok(_iMapper.Map<IEnumerable<PlatformReadDto>>(platformItem));
    }

    [HttpGet("{id}", Name = "GetPlatformById")]
    public ActionResult<PlatformReadDto> GetPlatformById(int id)
    {
        var platformItem = _iPlatformRepo.GetPlatFormById(id);

        if (platformItem is null)
        {
            return NotFound();
        }

        return Ok(_iMapper.Map<PlatformReadDto>(platformItem));
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platform)
    {
        var platformModel = _iMapper.Map<Platform>(platform);

        _iPlatformRepo.CreatePlatform(platformModel);

        _iPlatformRepo.SaveChange();

        var platformReadDto = _iMapper.Map<PlatformReadDto>(platformModel);

        //Send Sync Message
        try
        {
            await _commandDataClient.SendPlatformToCommand(platform);
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not send synchronously: {e.Message}");
        }

        //Send Async Message
        try
        {
            var platformPublishDto = _iMapper.Map<PlatformPublishDto>(platformReadDto);

            platformPublishDto.Event = "Platform_Published";

            _messageBusClient.PublishNewPlatform(platformPublishDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"--> Could not send asynchronously: {e.Message}");
        }

        return CreatedAtRoute(nameof(GetPlatformById), new {Id = platformReadDto.Id});
    }
}