namespace CommandsService.EventProcessing
{
    public interface IEventProcessorActionFactory
    {
        IEventProcessorAction? CreateAction(EventType eventType);
    }
}