using System.Runtime.CompilerServices;
using System.Threading.Channels;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Templahoot.Code;

public class CircuitTracker : BackgroundService
{
    private readonly QuizInfo _quiz;
    private readonly Channel<CircuitCommand> _commandChannel = Channel.CreateUnbounded<CircuitCommand>();

    public Dictionary<string, CircuitInfo> Circuits = new();
    public List<string> Reactions = new();
    public int? QuestionIndex = null;
    public QuestionInfo? CurrentQuestion => QuestionIndex.HasValue ? _quiz.Questions[QuestionIndex.Value] : null;
    public Timer QuestionTimer = new(1000);
    public DateTime? QuestionTimeOut;
    public bool QuestionReveal;

    public CircuitTracker(QuizInfo quiz)
    {
        _quiz = quiz;

        QuestionTimer.AutoReset = true;
        QuestionTimer.Elapsed += (o, e) => OnTimerElapsed().Wait();
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
                    break;
                }
                case CircuitClosed circuitClosed:
                {
                    if (Circuits.TryGetValue(circuitClosed.CircuitId, out var circuit))
                    {
                        Circuits.Remove(circuit.CircuitId);
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
                    }
                    break;
                }
                case CircuitReactHeart circuitReact:
                {
                    if (Circuits.TryGetValue(circuitReact.CircuitId, out var circuit))
                    {
                        Reactions.Add($"{circuit.Name}: <3");
                    }
                    break;
                }
                case StartQuiz:
                {
                    ShowQuestion(0);
                    OnClientChange?.Invoke(null);
                    break;
                }
                case NextQuestion:
                {
                    ShowQuestion(QuestionIndex.GetValueOrDefault() + 1);
                    OnClientChange?.Invoke(null);
                    break;
                }
                case TimerElapsed:
                {
                    break;
                }
                case QuestionTimerElapsed:
                {
                    QuestionTimeOut = null;
                    QuestionTimer.Stop();
                    QuestionReveal = true;
                    OnClientChange?.Invoke(null);
                    break;
                }
                case NameSet nameSet:
                {
                    if (Circuits.TryGetValue(nameSet.CircuitId, out var circuitInfo))
                    {
                        Circuits[nameSet.CircuitId] = circuitInfo with { Name = nameSet.Name };
                    }
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

                        var points = 0;
                        if (answerSubmitted.Answer.CorrectAnswer)
                        {
                            // https://support.kahoot.com/hc/en-us/articles/115002303908-How-points-work
                            var now = DateTime.UtcNow;
                            var responseTime = now - questionTimeOut.Value.AddSeconds(-20);
                            var dividedValue = 20 / Math.Max(0.01, responseTime.TotalSeconds);
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
                    }
                    break;
                }
            }

            OnHostChange?.Invoke();
        }
    }

    private void ShowQuestion(int index)
    {
        QuestionReveal = false;
        QuestionIndex = Math.Clamp(0, index, _quiz.Questions.Length - 1);
        QuestionTimer.Start();
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

    public AnswerInfo? GetAnsweredAnswer(string circuitId)
    {
        if (Circuits.TryGetValue(circuitId, out var circuitInfo) && circuitInfo.LastQuestionAnswered == QuestionIndex)
        {
            return circuitInfo.LastAnswer;
        }

        return null;
    }
}

public record StartQuiz() : CircuitCommand;
public record NextQuestion() : CircuitCommand;
public record TimerElapsed() : CircuitCommand;
public record QuestionTimerElapsed() : CircuitCommand;
public record AnswerSubmitted(string CircuitId, AnswerInfo Answer) : CircuitCommand;
public record NameSet(string CircuitId, string Name) : CircuitCommand;