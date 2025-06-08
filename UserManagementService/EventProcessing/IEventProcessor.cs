namespace UserManagementService.EventProcessing;

public interface IEventProcessor
{
    void ProcessEvent(string message);
}