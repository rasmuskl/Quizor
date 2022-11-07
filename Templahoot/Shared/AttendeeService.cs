using Templahoot.Code;

namespace Templahoot.Shared;

public class AttendeeService
{
    private readonly CircuitTracker _circuitTracker;

    public AttendeeService(CircuitTracker circuitTracker)
    {
        _circuitTracker = circuitTracker;
    }

    public string Name { get; set; }
    public string CircuitId { get; set; }
}