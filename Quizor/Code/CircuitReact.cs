namespace Quizor.Code;

public record CircuitReact(string CircuitId, ReactionType Type) : CircuitCommand;