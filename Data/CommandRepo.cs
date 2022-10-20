using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data
{
    public class CommandRepo : ICommandRepo
    {
        private readonly AppDbContext context;

        public CommandRepo(AppDbContext context)
        {
            this.context = context;
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Platform>> GetAllPlatforms()
        {
            return await context.Platforms.ToListAsync();
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

        public async Task<IEnumerable<Command>> GetCommandsForPlatform(int platformId)
        {
            return await context.Commands
                .Where(command => command.PlatformId == platformId)
                .OrderBy(command => command.Platform.Name)
                .ToListAsync();
        }

        public async Task<Command?> GetCommand(int platformId, int commandId)
        {
            return await context.Commands
                .Where(command => command.PlatformId == platformId && command.Id == commandId)
                .FirstOrDefaultAsync();
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