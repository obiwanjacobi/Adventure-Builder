namespace Jacobi.AdventureBuilder.GameContracts;

[GenerateSerializer, Immutable]
public sealed class GameCommand
{
    public GameCommand(string kind, string name, string description, string action)
    {
        Kind = kind;
        Name = name;
        Description = description;
        Action = action;
    }

    [Id(0)]
    public string Kind { get; }
    [Id(1)]
    public string Name { get; }
    [Id(2)]
    public string Description { get; }
    [Id(3)]
    public string Action { get; }
}

[GenerateSerializer, Immutable]
public sealed class GameCommandResult
{
    public GameCommandResult()
    { }

    public GameCommandResult(IPassageGrain passage)
    {
        Passage = passage;
    }

    [Id(0)]
    public IPassageGrain? Passage { get; }
}
