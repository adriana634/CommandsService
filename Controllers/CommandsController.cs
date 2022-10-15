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

        public CommandsController(ICommandRepo commandRepo, IMapper mapper)
        {
            this.commandRepo = commandRepo;
            this.mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"--> Hit GetCommandsForPlatform: {platformId}");

            var exists = commandRepo.PlatformExists(platformId);
            if (exists == false) return NotFound();

            var commands = commandRepo.GetCommandsForPlatform(platformId);

            var result = mapper.Map<IEnumerable<CommandReadDto>>(commands);
            return Ok(result);
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"--> Hit GetCommandForPlatform: {platformId} / {commandId}");

            var exists = commandRepo.PlatformExists(platformId);
            if (exists == false) return NotFound();

            var command = commandRepo.GetCommand(platformId, commandId);
            if (command is null) return NotFound();

            var result = mapper.Map<CommandReadDto>(command);
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
        {
            Console.WriteLine($"--> Hit CreateCommandForPlatform: {platformId}");

            var exists = commandRepo.PlatformExists(platformId);
            if (exists == false) return NotFound();

            var commandModel = mapper.Map<Command>(commandDto);

            commandRepo.CreateCommand(platformId, commandModel);
            commandRepo.SaveChanges();

            var commandReadDto = mapper.Map<CommandReadDto>(commandModel);
            return CreatedAtRoute(nameof(GetCommandForPlatform),
                new { PlatformId = commandReadDto.PlatformId, CommandId = commandReadDto.Id },
                commandReadDto);
        }
    }
}