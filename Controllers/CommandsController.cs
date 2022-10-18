using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo commandRepo;
        private readonly IMapper mapper;
        private readonly ILogger<CommandsController> logger;

        public CommandsController(ICommandRepo commandRepo, IMapper mapper, ILogger<CommandsController> logger)
        {
            this.commandRepo = commandRepo;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommandReadDto>>> GetCommandsForPlatform(int platformId)
        {
            logger.LogInformation("Hit GetCommandsForPlatform: {PlatformId}", platformId);

            var exists = commandRepo.PlatformExists(platformId);
            if (exists == false) return NotFound();

            var commands = await commandRepo.GetCommandsForPlatform(platformId);

            var result = mapper.Map<IEnumerable<CommandReadDto>>(commands);
            return Ok(result);
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public async Task<ActionResult<CommandReadDto>> GetCommandForPlatform(int platformId, int commandId)
        {
            logger.LogInformation("Hit GetCommandForPlatform: {PlatformId} / {CommandId}", platformId, commandId);

            var command = await commandRepo.GetCommand(platformId, commandId);
            if (command is null) return NotFound();

            var result = mapper.Map<CommandReadDto>(command);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<CommandReadDto>> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
        {
            logger.LogInformation("Hit CreateCommandForPlatform: {PlatformId}", platformId);

            var exists = commandRepo.PlatformExists(platformId);
            if (exists == false) return NotFound();

            var commandModel = mapper.Map<Command>(commandDto);

            commandRepo.CreateCommand(platformId, commandModel);
            await commandRepo.SaveChangesAsync();

            var commandReadDto = mapper.Map<CommandReadDto>(commandModel);
            return CreatedAtRoute(nameof(GetCommandForPlatform),
                new { PlatformId = commandReadDto.PlatformId, CommandId = commandReadDto.Id },
                commandReadDto);
        }
    }
}