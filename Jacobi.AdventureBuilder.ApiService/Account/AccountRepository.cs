using Jacobi.AdventureBuilder.ApiService.Data;

namespace Jacobi.AdventureBuilder.ApiService.Account;

internal interface IAccountRepository
{
    Task<Account> CreateAccountAsync(string name, string email, string? nickname, CancellationToken ct);
}

internal sealed class AccountRepository : IAccountRepository
{
    private readonly IDatabase _database;

    public AccountRepository(IDatabase database)
    {
        _database = database;
    }

    public async Task<Account> CreateAccountAsync(string name, string email, string? nickname, CancellationToken ct)
    {
        var account = new Account(
            AccountId: Guid.NewGuid(),
            Email: email,
            Name: name,
            NickName: nickname ?? name
        );

        return await _database.SaveAsync(account, ct);
    }
}
