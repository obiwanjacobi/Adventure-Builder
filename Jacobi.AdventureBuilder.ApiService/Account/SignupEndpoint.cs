using FastEndpoints;
using FluentValidation;

namespace Jacobi.AdventureBuilder.ApiService.Account;

internal sealed record SignupRequest(string Name, string? Nickname, string Email);
internal sealed record SignupResponse(string Id);

internal sealed class SignupValidator : Validator<SignupRequest>
{
    public SignupValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty();
        RuleFor(r => r.Email)
            .NotEmpty();
    }
}

internal sealed class SignupEndpoint : Endpoint<SignupRequest, SignupResponse>
{
    private readonly IAccountRepository _repository;

    public SignupEndpoint(IAccountRepository repository)
        => _repository = repository;

    public override void Configure()
    {
        Post("accounts/signup");
        AllowAnonymous();
    }

    public override async Task HandleAsync(SignupRequest req, CancellationToken ct)
    {
        var account = await _repository.CreateAccountAsync(req.Name, req.Email, req.Nickname, ct);
        var url = $"/accounts/{account.AccountId}";
        var taskSend = Send.CreatedAtAsync(url, null, new SignupResponse(account.AccountId.ToString()), cancellation: ct);
        var taskPublish = PublishAsync(new AccountCreatedEvent(account.AccountId), cancellation: ct);

        await Task.WhenAll(taskSend, taskPublish);
    }
}
