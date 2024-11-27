using FastEndpoints;
using Jacobi.AdventureBuilder.ApiService.Data;

namespace Jacobi.AdventureBuilder.ApiService.Account;

internal sealed record Account(Guid AccountId, string Name, string? NickName, string Email)
    : Data.Entity(Email), ILogicalPartition
{
    public static string ContainerName { get; } = nameof(Account);
}

internal sealed record AccountCreatedEvent(Guid accountId) : IEvent;
