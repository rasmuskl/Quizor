using Microsoft.AspNetCore.Components.Server.Circuits;
using Quizor.Shared;

namespace Quizor.Code;

public class TrackingCircuitHandler : CircuitHandler
{
    private readonly CircuitTracker _circuitTracker;
    private readonly AttendeeService _attendeeService;
    private static readonly Random Random = new();

    public TrackingCircuitHandler(CircuitTracker circuitTracker, AttendeeService attendeeService)
    {
        _circuitTracker = circuitTracker;
        _attendeeService = attendeeService;
    }

    public override async Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        var name = "random#" + Random.Next(1024);
        _attendeeService.CircuitId = circuit.Id;
        _attendeeService.Name = name;
        await _circuitTracker.PostCommand(new CircuitOpened(circuit.Id, name));
    }

    public override async Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        await _circuitTracker.PostCommand(new CircuitClosed(circuit.Id));
    }

    public override async Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        await _circuitTracker.PostCommand(new CircuitConnectionDown(circuit.Id));
    }

    public override async Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        await _circuitTracker.PostCommand(new CircuitConnectionUp(circuit.Id));
    }
}