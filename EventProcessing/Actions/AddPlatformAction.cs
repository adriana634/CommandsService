using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using System.Text.Json;

namespace CommandsService.EventProcessing.Actions
{
    public class AddPlatformAction : IEventProcessorAction
    {
        private readonly IServiceScopeFactory scopeFactory;

        public AddPlatformAction(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public void ProcessMessage(string message)
        {
            using var scope = scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
            var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AddPlatformAction>>();

            var platformPublished = JsonSerializer.Deserialize<PlatformPublishedDto>(message);

            try
            {
                var platform = mapper.Map<Platform>(platformPublished);

                if (repo.ExternalPlatformExists(platform.ExternalId) == false)
                {
                    repo.CreatePlatform(platform);
                    repo.SaveChangesAsync();

                    logger.LogInformation("Platform added!");
                }
                else
                {
                    logger.LogInformation("Platform already exists...");
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Could not add Platform to DB: {ExceptionMessage}", ex.Message);
            }
        }
    }
}
