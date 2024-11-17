namespace Jacobi.AdventureBuilder.GameContracts;

[GenerateSerializer, Immutable]
public sealed record PlayerInfo(string PlayerId, Guid AccountId, string Nickname);
