using Microsoft.AspNetCore.Components.Server.Circuits;
using Quizor.Shared;

namespace Quizor.Code;

public class TrackingCircuitHandler(CircuitTracker circuitTracker, AttendeeService attendeeService)
    : CircuitHandler
{
    private static readonly Random Random = new();

    public override async Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        var name = "random#" + Random.Next(1024);
        attendeeService.CircuitId = circuit.Id;
        attendeeService.Name = name;
        await circuitTracker.PostCommand(new CircuitOpened(circuit.Id, name));
    }

    public override async Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        await circuitTracker.PostCommand(new CircuitClosed(circuit.Id));
    }

    public override async Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        await circuitTracker.PostCommand(new CircuitConnectionDown(circuit.Id));
    }

    public override async Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        await circuitTracker.PostCommand(new CircuitConnectionUp(circuit.Id));
    }
}