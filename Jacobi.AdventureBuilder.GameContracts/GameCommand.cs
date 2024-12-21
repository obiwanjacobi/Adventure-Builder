namespace Jacobi.AdventureBuilder.GameContracts;

[GenerateSerializer, Immutable]
public sealed class GameCommandInfo
{
    public GameCommandInfo(string id, string kind, string name, string description)
    {
        Id = id;
        Kind = kind;
        Name = name;
        Description = description;
    }

    [Id(0)]
    public string Id { get; }
    [Id(1)]
    public string Kind { get; }
    [Id(2)]
    public string Name { get; }
    [Id(3)]
    public string Description { get; }
}

[GenerateSerializer, Immutable]
public sealed class GameCommand
{
    public GameCommand(string id, string kind, string action)
    {
        Id = id;
        Kind = kind;
        Action = action;
    }

    [Id(0)]
    public string Id { get; }
    [Id(1)]
    public string Kind { get; }
    [Id(2)]
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
