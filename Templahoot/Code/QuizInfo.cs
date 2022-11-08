namespace Templahoot.Code;

public class QuizInfo
{
    public QuestionInfo[] Questions { get;  } = new[]
    {
        new QuestionInfo("What is the result?<br/><pre>this is some random code that is long</pre>", new AnswerInfo[]
        {
            new("1"),
            new("2"),
            new("3"),
            new("4", true),
        }),
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

    public QuestionInfo[] Questions2 { get; } = new[]
    {
        new QuestionInfo("What is the result?<br/><pre>2 + 2</pre>", new AnswerInfo[]
        {
            new("3"),
            new("4", true),
            new("5"),
            new("SyntaxError"),
        }, true),

        new QuestionInfo("What is the result?<br/><pre>1 + 2 + \"3\"</pre>", new AnswerInfo[]
        {
            new("\"123\""),
            new("\"33\"", true),
            new("NaN"),
            new("6"),
        }),

        new QuestionInfo("What is the result?<br/><pre>5 && 3</pre>", new AnswerInfo[]
        {
            new("true"),
            new("5"),
            new("SyntaxError"),
            new("3", true),
        }),

        new QuestionInfo("What is the result?<br/><pre>42.toString()</pre>", new AnswerInfo[]
        {
            new("\"42\""),
            new("\"41.99999999999\""),
            new("SyntaxError", true),
            new("[object Object]"),
        }),

        new QuestionInfo("What is the result?<br/><pre>010 - 01</pre>", new AnswerInfo[]
        {
            new("1"),
            new("9"),
            new("7", true),
            new("NaN"),
        }),

        new QuestionInfo("What is the result?<br/><pre>[,,,,].length</pre>", new AnswerInfo[]
        {
            new("4"),
            new("SyntaxError"),
            new("0"),
            new("3", true),
        }),

        new QuestionInfo("What is the result?<br/><pre>[3, 2, 1] + [4, 5, 6]</pre>", new AnswerInfo[]
        {
            new("[3, 2, 1, 4, 5, 6]"),
            new("\"3,2,14,5,6\"", true),
            new("SyntaxError"),
            new("0"),
        }),

        new QuestionInfo("What is the result?<br/><pre>new Date(2022, 5, 31).toDateString()</pre>", new AnswerInfo[]
        {
            new("\"Thu Jun 31 2022\""),
            new("\"Invalid date\""),
            new("\"Fri Jul 01 2022\"", true),
            new("\"Tue May 31 2022\""),
        }),

        new QuestionInfo("What is the result?<br/><pre>{} + []</pre>", new AnswerInfo[]
        {
            new("0", true),
            new("NaN"),
            new("SyntaxError"),
            new("\"[object Object]\""),
        }),

        //new QuestionInfo("What is the result?<br/><pre>[] + {}</pre>", new AnswerInfo[]
        //{
        //    new("0"),
        //    new("NaN"),
        //    new("SyntaxError"),
        //    new("\"[object Object]\"", true),
        //}),

        new QuestionInfo("What is the result?<br/><pre>{} - []</pre>", new AnswerInfo[]
        {
            new("0"),
            new("NaN", true),
            new("SyntaxError"),
            new("\"[object Object]\""),
        }),

        new QuestionInfo("What is the result?<br/><pre><!-- 42</pre>", new AnswerInfo[]
        {
            new("undefined", true),
            new("42"),
            new("SyntaxError"),
            new("\"0\""),
        }),

        new QuestionInfo("What is the result?<br/><pre>{ ignite: 2022 }.ignite</pre>", new AnswerInfo[]
        {
            new("2022"),
            new("ReferenceError"),
            new("SyntaxError", true),
            new("undefined"),
        }),

        new QuestionInfo("What is the result?<br/><pre>\"\" - - \"\"</pre>", new AnswerInfo[]
        {
            new("0", true),
            new("SyntaxError"),
            new("\"\""),
            new("undefined"),
        }),
    };
}

public record QuestionInfo(string Text, AnswerInfo[] Answers, bool NoPoints = false);

public record AnswerInfo(string Text, bool CorrectAnswer = false);
