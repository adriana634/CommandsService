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
            return (context.SaveChanges() >= 0);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return context.Platforms.ToList();
        }

        public void CreatePlatform(Platform platform)
        {
            if (platform is null)
            {
                throw new ArgumentNullException(nameof(platform));
            }

            context.Platforms.Add(platform);
        }

        public bool PlatformExists(int platformId)
        {
            return context.Platforms.Any(platform => platform.Id == platformId);
        }

        public bool ExternalPlatformExists(int externalPlatformId)
        {
            return context.Platforms.Any(platform => platform.ExternalId == externalPlatformId);
        }

        public IEnumerable<Command> GetCommandsForPlatform(int platformId)
        {
            return context.Commands
                .Where(command => command.PlatformId == platformId)
                .OrderBy(command => command.Platform.Name);
        }

        public Command? GetCommand(int platformId, int commandId)
        {
            return context.Commands
                .Where(command => command.PlatformId == platformId && command.Id == commandId)
                .FirstOrDefault();
        }

        public void CreateCommand(int platformId, Command command)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            command.PlatformId = platformId;
            context.Commands.Add(command);
        }
    }
}