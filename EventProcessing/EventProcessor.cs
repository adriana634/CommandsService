using CommandsService.Dtos;
using System.Text.Json;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IEventProcessorActionFactory eventProcessorActionFactory;
        private readonly ILogger<EventProcessor> logger;

        public EventProcessor(IEventProcessorActionFactory eventProcessorActionFactory, ILogger<EventProcessor> logger)
        {
            this.eventProcessorActionFactory = eventProcessorActionFactory;
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

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEventType(message);
            var action = eventProcessorActionFactory.CreateAction(eventType);
            action!.ProcessMessage(message);
        }
    }
}