namespace Quizor.Code;

public class QuizInfo
{
    public QuestionInfo[] Questions { get; } =
    [
        new("What is the result?<br/><pre>2 + 2</pre>", [
            new AnswerInfo("3"),
            new AnswerInfo("4", true),
            new AnswerInfo("5"),
            new AnswerInfo("SyntaxError")
        ], true),

        new("How many l's in lollipop?", [
            new AnswerInfo("2"),
            new AnswerInfo("3", true),
            new AnswerInfo("4"),
            new AnswerInfo("5")
        ]),
        
        new("What are the release channel order for VSTO add-ins?", [
            new AnswerInfo("Staging-Preview-Production"),
            new AnswerInfo("Insider-Internal-Stable"),
            new AnswerInfo("Internal-Preview-Stable"),
            new AnswerInfo("Internal-Insider-Stable", true)
        ]),

        new("Which of these is NOT the name of a Templafy custom beer?", [
            new AnswerInfo("Macrobrew", true),
            new AnswerInfo("PowerPint"),
            new AnswerInfo("Final Draft"),
            new AnswerInfo("Anarchy Hopped")
        ]),

        new("What is the JavaScript result?<br/><pre>undefined + false</pre>", [
            new AnswerInfo("false"),
            new AnswerInfo("\"0\""),
            new AnswerInfo("NaN", true),
            new AnswerInfo("SyntaxError")
        ]),

        new("What is the hardest thing to roll out in Templafy?", [
            new AnswerInfo("Web add-ins manifests"),
            new AnswerInfo("Templafy Desktop Host", true),
            new AnswerInfo("VSTO add-ins 6.x offline mode"),
            new AnswerInfo("Templafy One")
        ]),

        new("Which iteration of Ignite is Ignite 2025?", [
            new AnswerInfo("3"),
            new AnswerInfo("4"),
            new AnswerInfo("5", true),
            new AnswerInfo("6")
        ]),

        new("How did we improve narrative extraction in Conversational AI last week?", [
            new AnswerInfo("Offer money"),
            new AnswerInfo("More specific instructions"),
            new AnswerInfo("Split task in 2 prompts"),
            new AnswerInfo("Threaten with jail", true),
        ]),

        new("What was the worst incident in Templafy history?", [
            new AnswerInfo("Templafy Desktop timeout retry DDoS"),
            new AnswerInfo("Manual UPDATE without WHERE"),
            new AnswerInfo("App Service scaling IP change"),
            new AnswerInfo("Deprecated Timestamp Server", true),
        ]),

        new("Who has won most Ignite hackathon awards in total?", [
            new AnswerInfo("Tiago"),
            new AnswerInfo("Ibrahim", true),
            new AnswerInfo("Jacob"),
            new AnswerInfo("Mehmet")
        ]),

        new("What is the JavaScript result?<br/><pre>`${parseInt(0.2)}, ${parseInt(0.0000002)}`</pre>", [
            new AnswerInfo("\"0, 0\""),
            new AnswerInfo("\"2, 0\""),
            new AnswerInfo("\"0, 2\"", true),
            new AnswerInfo("\"2, 2\"")
        ]),

        new("Which virtual meeting room would you use for gnarly issues before Hive?", [
            new AnswerInfo("Narnia"),
            new AnswerInfo("Chamber of Secrets"),
            new AnswerInfo("Mordor", true),
            new AnswerInfo("The Bat Cave"),
        ]),

        new("At Templafy Ignite, an AI decides to invent a new office mascot. What does it name the mascot?", [
            new AnswerInfo("Contentron Prime"),
            new AnswerInfo("Sir Merge-a-Lot"),
            new AnswerInfo("DocuDuke 3000"),
            new AnswerInfo("Slidey McTemplateface", true),
        ]),
    ];
}

public record QuestionInfo(string Text, AnswerInfo[] Answers, bool NoPoints = false);

public record AnswerInfo(string Text, bool CorrectAnswer = false);
