using CommandsService.Models;

namespace CommandsService.Data
{
    public interface ICommandRepo
    {
        void SaveChanges();
        Task SaveChangesAsync();

        // Platforms
        Task<IEnumerable<Platform>> GetAllPlatforms();
        void CreatePlatform(Platform platform);
        bool PlatformExists(int platformId);
        bool ExternalPlatformExists(int externalPlatformId);

        // Commands
        Task<IEnumerable<Command>> GetCommandsForPlatform(int platformId);
        Task<Command?> GetCommand(int platformId, int commandId);
        void CreateCommand(int platformId, Command command);
    }
}