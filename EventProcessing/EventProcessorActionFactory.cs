using AutoMapper;
using CommandsService.EventProcessing.Actions;

namespace CommandsService.EventProcessing
{
    public class EventProcessorActionFactory : IEventProcessorActionFactory
    {
        private readonly IServiceScopeFactory scopeFactory;

        public EventProcessorActionFactory(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public IEventProcessorAction CreateAction(EventType eventType)
        {
            return eventType switch
            {
                EventType.PlatformPublished => new AddPlatformAction(scopeFactory),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
