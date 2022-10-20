using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data
{
    public class PrepDb
    {
        private static void SeedData(ICommandRepo commandRepo, ILogger<PrepDb> logger, IEnumerable<Platform> platforms)
        {
            logger.LogInformation("Seeding new platforms...");

            foreach (var platform in platforms)
            {
                if (commandRepo.ExternalPlatformExists(platform.ExternalId) == false)
                {
                    commandRepo.CreatePlatform(platform);
                }
            }

            commandRepo.SaveChanges();
        }

        public static void PrepPopulation(IApplicationBuilder applicationBuilder)
        {
            using (var scope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var grpcClient = scope.ServiceProvider.GetRequiredService<IPlatformDataClient>();
                var commandRepo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<PrepDb>>();

                var platforms = grpcClient.GetAllPlatforms();
                SeedData(commandRepo, logger, platforms);
            }
        }
    }
}