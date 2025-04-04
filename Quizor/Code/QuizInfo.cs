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

        new("What is the result?<br/><pre>1 + 2 + \"3\"</pre>", [
            new AnswerInfo("\"123\""),
            new AnswerInfo("\"33\"", true),
            new AnswerInfo("NaN"),
            new AnswerInfo("6")
        ]),

        new("What is the result?<br/><pre>5 && 3</pre>", [
            new AnswerInfo("true"),
            new AnswerInfo("5"),
            new AnswerInfo("SyntaxError"),
            new AnswerInfo("3", true)
        ]),

        new("What is the result?<br/><pre>42.toString()</pre>", [
            new AnswerInfo("\"42\""),
            new AnswerInfo("\"41.99999999999\""),
            new AnswerInfo("SyntaxError", true),
            new AnswerInfo("[object Object]")
        ]),

        new("What is the result?<br/><pre>010 - 01</pre>", [
            new AnswerInfo("1"),
            new AnswerInfo("9"),
            new AnswerInfo("7", true),
            new AnswerInfo("NaN")
        ]),

        new("What is the result?<br/><pre>[,,,,].length</pre>", [
            new AnswerInfo("4"),
            new AnswerInfo("SyntaxError"),
            new AnswerInfo("0"),
            new AnswerInfo("3", true)
        ]),

        new("What is the result?<br/><pre>[3, 2, 1] + [4, 5, 6]</pre>", [
            new AnswerInfo("[3, 2, 1, 4, 5, 6]"),
            new AnswerInfo("\"3,2,14,5,6\"", true),
            new AnswerInfo("SyntaxError"),
            new AnswerInfo("0")
        ]),

        new("What is the result?<br/><pre>new Date(2022, 5, 31).toDateString()</pre>", [
            new AnswerInfo("\"Thu Jun 31 2022\""),
            new AnswerInfo("\"Invalid date\""),
            new AnswerInfo("\"Fri Jul 01 2022\"", true),
            new AnswerInfo("\"Tue May 31 2022\"")
        ]),

        new("What is the result?<br/><pre>{} + []</pre>", [
            new AnswerInfo("0", true),
            new AnswerInfo("NaN"),
            new AnswerInfo("SyntaxError"),
            new AnswerInfo("\"[object Object]\"")
        ]),

        //new QuestionInfo("What is the result?<br/><pre>[] + {}</pre>", new AnswerInfo[]
        //{
        //    new("0"),
        //    new("NaN"),
        //    new("SyntaxError"),
        //    new("\"[object Object]\"", true),
        //}),

        new("What is the result?<br/><pre>{} - []</pre>", [
            new AnswerInfo("0"),
            new AnswerInfo("NaN", true),
            new AnswerInfo("SyntaxError"),
            new AnswerInfo("\"[object Object]\"")
        ]),

        new("What is the result?<br/><pre>&lt;!-- 42</pre>", [
            new AnswerInfo("undefined", true),
            new AnswerInfo("42"),
            new AnswerInfo("SyntaxError"),
            new AnswerInfo("\"0\"")
        ]),

        new("What is the result?<br/><pre>{ ignite: 2022 }.ignite</pre>", [
            new AnswerInfo("2022"),
            new AnswerInfo("ReferenceError"),
            new AnswerInfo("SyntaxError", true),
            new AnswerInfo("undefined")
        ]),

        new("What is the result?<br/><pre>\"\" - - \"\"</pre>", [
            new AnswerInfo("0", true),
            new AnswerInfo("SyntaxError"),
            new AnswerInfo("\"\""),
            new AnswerInfo("undefined")
        ])
    ];
}

public record QuestionInfo(string Text, AnswerInfo[] Answers, bool NoPoints = false);

public record AnswerInfo(string Text, bool CorrectAnswer = false);
