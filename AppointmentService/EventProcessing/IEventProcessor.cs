namespace AppointmentService.EventProcessing;

public interface IEventProcessor
{
    void ProcessEvent(string message, string type);
}