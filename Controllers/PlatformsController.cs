using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly ICommandRepo commandRepo;
        private readonly IMapper mapper;

        public PlatformsController(ICommandRepo commandRepo, IMapper mapper)
        {
            this.commandRepo = commandRepo;
            this.mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("--> Getting Platforms from CommandsService");

            IEnumerable<Platform> platforms = this.commandRepo.GetAllPlatforms();
            
            IEnumerable<PlatformReadDto> result = this.mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
            return Ok(result);
        }

        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("--> Inbound POST # Command Service");
            return Ok("Inbound test from Platform Controller");
        }
    }
}