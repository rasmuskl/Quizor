using System.Collections.Concurrent;
using System.Threading.Channels;
using Timer = System.Timers.Timer;

namespace Quizor.Code;

public class CircuitTracker : BackgroundService
{
    private readonly Channel<CircuitCommand> _commandChannel = Channel.CreateUnbounded<CircuitCommand>();
    private readonly QuizInfo _quiz;

    public readonly ConcurrentDictionary<string, CircuitInfo> Circuits = new();
    public readonly ConcurrentQueue<AttendeeReaction> Reactions = new();
    public int? QuestionIndex;
    public QuestionInfo? CurrentQuestion => QuestionIndex.HasValue ? _quiz.Questions[QuestionIndex.Value] : null;
    private readonly Timer _questionTimer = new(1000);
    public DateTime? QuestionTimeOut;
    public bool QuestionReveal;
    public List<QuestionAnswer> LastQuestionAnswers = [];
    
    public int QuestionMaxIndex => _quiz.Questions.Length - 1;

    public CircuitTracker(QuizInfo quiz)
    {
        _quiz = quiz;
        _questionTimer.AutoReset = true;
        _questionTimer.Elapsed += (_, _) => OnTimerElapsed().Wait();
    }

    private async Task OnTimerElapsed()
    {
        if (!QuestionTimeOut.HasValue || DateTime.UtcNow > QuestionTimeOut)
        {
            await PostCommand(new QuestionTimerElapsed());
        }
        else
        {
            await PostCommand(new TimerElapsed());
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var command in _commandChannel.Reader.ReadAllAsync(stoppingToken))
        {
            switch (command)
            {
                case CircuitOpened circuitOpened:
                {
                    if (Circuits.TryGetValue(circuitOpened.CircuitId, out var circuitInfo))
                    {
                        Circuits[circuitOpened.CircuitId] = circuitInfo with
                        {
                            Name = circuitOpened.Name, 
                            State = CircuitState.Connected
                        };
                    }
                    else
                    {
                        Circuits.TryAdd(circuitOpened.CircuitId, new CircuitInfo(circuitOpened.CircuitId, circuitOpened.Name, CircuitState.Connected, 0, -1, null));
                    }
                    OnHostChange?.Invoke();
                    break;
                }
                case CircuitClosed circuitClosed:
                {
                    if (Circuits.TryRemove(circuitClosed.CircuitId, out _))
                    {
                        OnHostChange?.Invoke();
                    }
                    break;
                }
                case CircuitConnectionDown circuitConnectionDown:
                {
                    if (Circuits.TryGetValue(circuitConnectionDown.CircuitId, out var circuit))
                    {
                        Circuits[circuit.CircuitId] = Circuits[circuit.CircuitId] with
                        {
                            State = CircuitState.Disconnected
                        };
                        OnHostChange?.Invoke();
                    }
                    break;
                }
                case CircuitConnectionUp circuitConnectionUp:
                {
                    if (Circuits.TryGetValue(circuitConnectionUp.CircuitId, out var circuit))
                    {
                        Circuits[circuit.CircuitId] = Circuits[circuit.CircuitId] with
                        {
                            State = CircuitState.Connected
                        };
                        OnHostChange?.Invoke();
                    }
                    break;
                }
                case CircuitReact circuitReact:
                {
                    if (Circuits.TryGetValue(circuitReact.CircuitId, out var circuit))
                    {
                        Reactions.Enqueue(new AttendeeReaction(circuit.Name, circuitReact.Type));

                        // Make sure we don't have an infinite loop
                        var breaker = 0;

                        while (Reactions.Count > 20 && breaker < 50)
                        {
                            Reactions.TryDequeue(out _);
                            breaker++;
                        }

                        OnHostChange?.Invoke();
                    }
                    break;
                }
                case StartQuiz:
                {
                    ShowQuestion(0);
                    OnClientChange?.Invoke(null);
                    OnHostChange?.Invoke();
                    break;
                }
                case NextQuestion:
                {
                    ShowQuestion(QuestionIndex.GetValueOrDefault() + 1);
                    LastQuestionAnswers = [];
                    OnClientChange?.Invoke(null);
                    OnHostChange?.Invoke();
                    break;
                }
                case TimerElapsed:
                {
                    OnHostChange?.Invoke();
                    break;
                }
                case QuestionTimerElapsed:
                {
                    QuestionTimeOut = null;
                    _questionTimer.Stop();
                    QuestionReveal = true;
                    OnClientChange?.Invoke(null);
                    OnHostChange?.Invoke();
                    break;
                }
                case NameSet nameSet:
                {
                    if (Circuits.TryGetValue(nameSet.CircuitId, out var circuitInfo))
                    {
                        Circuits[nameSet.CircuitId] = circuitInfo with { Name = nameSet.Name };
                    }
                    OnHostChange?.Invoke();
                    break;
                }
                case AnswerSubmitted answerSubmitted:
                {
                    var questionTimeOut = QuestionTimeOut;
                    if (!questionTimeOut.HasValue)
                    {
                        break;
                    }

                    if (Circuits.TryGetValue(answerSubmitted.CircuitId, out var circuitInfo))
                    {
                        if (circuitInfo.LastQuestionAnswered == QuestionIndex.GetValueOrDefault())
                        {
                            break;
                        }
                        
                        LastQuestionAnswers.Add(new QuestionAnswer(answerSubmitted.CircuitId, circuitInfo.Name, answerSubmitted.Answer));

                        var points = 0;
                        if (answerSubmitted.Answer.CorrectAnswer && CurrentQuestion?.NoPoints != true)
                        {
                            // https://support.kahoot.com/hc/en-us/articles/115002303908-How-points-work
                            var now = DateTime.UtcNow;
                            var responseTime = now - questionTimeOut.Value.AddSeconds(-20);
                            var dividedValue = responseTime.TotalSeconds / 20;
                            var dividedAgain = dividedValue / 2;
                            var subtracted = 1 - dividedAgain;
                            var multiplied = subtracted * 1000;
                            points = (int)Math.Round(multiplied);
                        }

                        Circuits[answerSubmitted.CircuitId] = circuitInfo with
                        {
                            Points = circuitInfo.Points + points,
                            LastQuestionAnswered = QuestionIndex.GetValueOrDefault(),
                            LastAnswer = answerSubmitted.Answer,
                        };

                        OnClientChange?.Invoke(answerSubmitted.CircuitId);
                        OnHostChange?.Invoke();
                    }
                    break;
                }
            }
        }
    }

    private void ShowQuestion(int index)
    {
        QuestionReveal = false;
        QuestionIndex = Math.Clamp(0, index, _quiz.Questions.Length - 1);
        _questionTimer.Start();
        QuestionTimeOut = DateTime.UtcNow.AddSeconds(20);
    }

    public event Func<Task>? OnHostChange;
    public event Func<string?, Task>? OnClientChange;

    public async Task PostCommand(CircuitCommand command)
    {
        await _commandChannel.Writer.WriteAsync(command);
    }

    public int GetPoints(string circuitId)
    {
        if (string.IsNullOrWhiteSpace(circuitId))
        {
            return 0;
        }

        if (Circuits.TryGetValue(circuitId, out var circuitInfo))
        {
            return circuitInfo.Points;
        }

        return 0;
    }

    public AnswerInfo? GetAnsweredAnswer(string? circuitId)
    {
        if (string.IsNullOrWhiteSpace(circuitId))
        {
            return null;
        }

        if (Circuits.TryGetValue(circuitId, out var circuitInfo) && circuitInfo.LastQuestionAnswered == QuestionIndex)
        {
            return circuitInfo.LastAnswer;
        }

        return null;
    }
}

public record StartQuiz : CircuitCommand;
public record NextQuestion : CircuitCommand;
public record TimerElapsed : CircuitCommand;
public record QuestionTimerElapsed : CircuitCommand;
public record AnswerSubmitted(string CircuitId, AnswerInfo Answer) : CircuitCommand;
public record NameSet(string CircuitId, string Name) : CircuitCommand;

public record QuestionAnswer(string CircuitId, string Name, AnswerInfo Answer);

public record AttendeeReaction(string Name, ReactionType Type);

public enum ReactionType
{
    Heart,
    Joy,
    Explode,
    Think,
    Invader,
}