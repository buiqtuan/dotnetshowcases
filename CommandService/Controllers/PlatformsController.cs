using AutoMapper;
using CommandService.Data;
using Microsoft.AspNetCore.Mvc;
using CommandService.Dtos;

namespace CommandService;

[ApiController]
[Route("api/c/[controller]")]
public class PlatformsController : ControllerBase
{
    private readonly ICommandRepo _commandRepo;

    private readonly IMapper _mapper;

    public PlatformsController(ICommandRepo repository, IMapper mapper)
    {
        _commandRepo = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        Console.WriteLine("--> Getting platforms from ComandService");

        var platformItems = _commandRepo.GetAllPlatforms();

        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
    } 

    [HttpPost]
    public ActionResult TestInboundConnection()
    {
        Console.WriteLine("--> Inbound POST # Command Service");

        return Ok("Inbound test ok from Platforms Controller");
    }
}