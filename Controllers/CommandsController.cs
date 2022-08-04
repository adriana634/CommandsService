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

            bool exists = this.commandRepo.PlatformExists(platformId);
            if (exists == false) return NotFound();

            IEnumerable<Command> commands = this.commandRepo.GetCommandsForPlatform(platformId);

            IEnumerable<CommandReadDto> result = this.mapper.Map<IEnumerable<CommandReadDto>>(commands);
            return Ok(result);
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"--> Hit GetCommandForPlatform: {platformId} / {commandId}");

            bool exists = this.commandRepo.PlatformExists(platformId);
            if (exists == false) return NotFound();

            Command command = this.commandRepo.GetCommand(platformId, commandId);
            if (command is null) return NotFound();

            CommandReadDto result = this.mapper.Map<CommandReadDto>(command);
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
        {
            Console.WriteLine($"--> Hit CreateCommandForPlatform: {platformId}");

            bool exists = this.commandRepo.PlatformExists(platformId);
            if (exists == false) return NotFound();

            Command commandModel = this.mapper.Map<Command>(commandDto);

            this.commandRepo.CreateCommand(platformId, commandModel);
            this.commandRepo.SaveChanges();

            CommandReadDto commandReadDto = this.mapper.Map<CommandReadDto>(commandModel);
            return CreatedAtRoute(nameof(GetCommandForPlatform),
                new { PlatformId = commandReadDto.PlatformId, CommandId = commandReadDto.Id },
                commandReadDto);
        }
    }
}