﻿namespace Jacobi.AdventureBuilder.GameContracts;

[GenerateSerializer, Immutable]
public sealed class GameCommand
{
    public GameCommand(string kind, string name, string description, string action, string displayName, string subject)
    {
        Kind = kind;
        Name = name;
        Description = description;
        Action = action;
        DisplayName = displayName;
        Subject = subject;
    }

    [Id(0)]
    public string Kind { get; }
    [Id(1)]
    public string Name { get; }
    [Id(2)]
    public string Description { get; }
    [Id(3)]
    public string Action { get; }
    [Id(4)]
    public string DisplayName { get; }
    [Id(5)]
    public string Subject { get; }
}

[GenerateSerializer, Immutable]
public sealed class GameCommandResult
{
    public GameCommandResult()
    { }

    public GameCommandResult(IPassageGrain passage)
    {
        Success = true;
        Passage = passage;
    }

    [Id(0)]
    public bool Success { get; }

    [Id(1)]
    public IPassageGrain? Passage { get; }
}
