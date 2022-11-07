namespace Templahoot.Code;

public class QuizInfo
{
    public QuestionInfo[] Questions { get;  } = new[]
    {
        new QuestionInfo("What is 2 + 2?", new AnswerInfo[]
        {
            new("1"),
            new("2"),
            new("3"),
            new("4", true),
        }),
        new QuestionInfo("What is the fastest animal?", new AnswerInfo[]
        {
            new("Cheetah"),
            new("Oviraptor"),
            new("Elephant"),
            new("Snail", true),
        }),
        new QuestionInfo("What is 2 / 2?", new AnswerInfo[]
        {
            new("1", true),
            new("2"),
            new("3"),
            new("4"),
        }),
    };
}

public record QuestionInfo(string Text, AnswerInfo[] Answers);

public record AnswerInfo(string Text, bool CorrectAnswer = false);
