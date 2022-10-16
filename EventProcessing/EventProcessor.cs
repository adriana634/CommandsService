using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IMapper mapper;
        private readonly ILogger<EventProcessor> logger;

        private enum EventType
        {
            Undetermined,
            PlatformPublished,
        }

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper, ILogger<EventProcessor> logger)
        {
            this.scopeFactory = scopeFactory;
            this.mapper = mapper;
            this.logger = logger;
        }

        private EventType DetermineEventType(string notificationMessage)
        {
            logger.LogInformation("Determining Event");

            try
            {
                var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

                if (eventType is not null)
                {
                    switch (eventType.Event)
                    {
                        case "Platform_Published":
                            logger.LogInformation("Platform Published Event Detected");
                            return EventType.PlatformPublished;
                        default:
                            logger.LogInformation("Could not determine the event type");
                            return EventType.Undetermined;
                    }
                }
                else
                {
                    return EventType.Undetermined;
                }
            }
            catch (Exception)
            {
                return EventType.Undetermined;
            }
        }

        private void AddPlatform(string platformPublishedMessage)
        {
            using var scope = scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

            var platformPublished = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

            try
            {
                var platform = mapper.Map<Platform>(platformPublished);

                if (repo.ExternalPlatformExists(platform.ExternalId) == false)
                {
                    repo.CreatePlatform(platform);
                    repo.SaveChanges();

                    logger.LogInformation("Platform added!");
                }
                else
                {
                    logger.LogInformation("Platform already exists...");
                }
            }
            catch(Exception ex)
            {
                logger.LogError("Could not add Platform to DB: {ExceptionMessage}", ex.Message);
            }
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEventType(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
            }
        }
    }
}