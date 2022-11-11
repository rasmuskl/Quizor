namespace Quizor.Code;

public record CircuitInfo(string CircuitId, string Name, CircuitState State, int Points, int LastQuestionAnswered, AnswerInfo? LastAnswer);