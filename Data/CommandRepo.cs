using CommandsService.Models;

namespace CommandsService.Data
{
    public class CommandRepo : ICommandRepo
    {
        private readonly AppDbContext context;

        public CommandRepo(AppDbContext context)
        {
            this.context = context;
        }

        public bool SaveChanges()
        {
            return (this.context.SaveChanges() >= 0);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return this.context.Platforms.ToList();
        }

        public void CreatePlatform(Platform platform)
        {
            if (platform is null)
            {
                throw new ArgumentNullException(nameof(platform));
            }

            this.context.Platforms.Add(platform);
        }

        public bool PlatformExists(int platformId)
        {
            return this.context.Platforms.Any(platform => platform.Id == platformId);
        }

        public IEnumerable<Command> GetCommandsForPlatform(int platformId)
        {
            return this.context.Commands
                .Where(command => command.PlatformId == platformId)
                .OrderBy(command => command.Platform.Name);
        }

        public Command GetCommand(int platformId, int commandId)
        {
            return this.context.Commands
                .Where(command => command.PlatformId == platformId && command.Id == commandId)
                .First();
        }

        public void CreateCommand(int platformId, Command command)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            command.PlatformId = platformId;
            this.context.Commands.Add(command);
        }
    }
}