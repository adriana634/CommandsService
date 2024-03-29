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
        private readonly ILogger<PlatformsController> logger;

        public PlatformsController(ICommandRepo commandRepo, IMapper mapper, ILogger<PlatformsController> logger)
        {
            this.commandRepo = commandRepo;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlatformReadDto>>> GetPlatforms()
        {
            logger.LogInformation("Getting Platforms from CommandsService");

            var platforms = await commandRepo.GetAllPlatforms();

            var result = mapper.Map<IEnumerable<PlatformReadDto>>(platforms);
            return Ok(result);
        }

        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            logger.LogInformation("Inbound POST # Command Service");
            return Ok("Inbound test from Platform Controller");
        }
    }
}