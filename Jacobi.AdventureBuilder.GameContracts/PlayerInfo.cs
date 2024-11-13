namespace Jacobi.AdventureBuilder.GameContracts;

[GenerateSerializer, Immutable]
public sealed record PlayerInfo(Guid GrainId, Guid AccountId, string Nickname);
