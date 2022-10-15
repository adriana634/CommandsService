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

        private enum EventType
        {
            Undetermined,
            PlatformPublished,
        }

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            this.scopeFactory = scopeFactory;
            this.mapper = mapper;
        }

        private EventType DetermineEventType(string notificationMessage)
        {
            Console.WriteLine("--> Determining Event");

            try
            {
                var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

                if (eventType is not null)
                {
                    switch (eventType.Event)
                    {
                        case "Platform_Published":
                            Console.WriteLine("--> Platform Published Event Detected");
                            return EventType.PlatformPublished;
                        default:
                            Console.WriteLine("--> Could not determine the event type");
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
                }
                else
                {
                    Console.WriteLine("--> Platform already exists...");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"--> Could not add Platform to DB {ex.Message}");
            }
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEventType(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    break;
                default:
                    break;
            }
        }
    }
}