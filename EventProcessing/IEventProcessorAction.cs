namespace CommandsService.EventProcessing
{
    public interface IEventProcessorAction
    {
        public void ProcessMessage(string message);
    }
}
