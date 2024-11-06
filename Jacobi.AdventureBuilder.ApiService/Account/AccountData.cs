using FastEndpoints;

namespace Jacobi.AdventureBuilder.ApiService.Account;

internal sealed record Account(Guid AccountId, string Name, string? NickName, string Email)
    : Data.Entity(Email);

internal sealed record AccountCreatedEvent(Guid accountId) : IEvent;
