using CommandService.Data;
using CommandService.Models;
using CommandService.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[Route("api/c/platforms/{platformId}/[controller]")]
[ApiController]
public class CommandsController : ControllerBase
{
    private readonly ICommandRepo _repository;

    private readonly IMapper _mapper;

    public CommandsController(ICommandRepo repository, IMapper mapper)
    {
        _repository = repository; 
        _mapper = mapper;  
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommandsFroPlatform(int platformId)
    {
        Console.WriteLine("--> Getting commands from platformId");

        if (!_repository.PlatformExits(platformId))
        {
            return NotFound();
        }

        return Ok(_mapper.Map<IEnumerable<CommandReadDto>>
            (
                _repository.GetCommandsForPlatform(platformId)
            ));
    }

    [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
    public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
    {
        if (!_repository.PlatformExits(platformId))
        {
            return NotFound($"PlatformId {platformId} not found!");
        }

        var command = _repository.GetCommand(platformId, commandId);

        if (command is null)
        {
            return NotFound($"No command for platformId {platformId} found");
        }

        return Ok(_mapper.Map<CommandReadDto>(command));
    }

    [HttpPost]
    public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
    {
        if (!_repository.PlatformExits(platformId))
        {
            return NotFound($"PlatformId {platformId} not found!");
        }

        var command = _mapper.Map<Command>(commandDto);

        _repository.CreateCommand(platformId, command);
        _repository.SaveChange();

        var commandReadDto =  _mapper.Map<CommandReadDto>(command);

        return CreatedAtRoute(nameof(GetCommandForPlatform), new {platformId, commandId = commandReadDto.Id, commandReadDto});
    }
}