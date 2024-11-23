using FastEndpoints;
using FluentValidation;
using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.ApiService.Adventure;

internal sealed record AdventureWorldRequest(string WorldId);

internal sealed class AdventureWorldValidator : Validator<AdventureWorldRequest>
{
    public AdventureWorldValidator()
    {
        RuleFor(r => r.WorldId)
            .NotEmpty();
    }
}

internal sealed class AdventureWorldEndpoint : Endpoint<AdventureWorldRequest, AdventureWorldInfo>
{
    private readonly IAdventureRepository _repository;

    public AdventureWorldEndpoint(IAdventureRepository repository)
        => _repository = repository;

    public override void Configure()
    {
        Get("adventure/worlds/{WorldId}");
        // TODO: authentication
        AllowAnonymous();
    }

    public override async Task HandleAsync(AdventureWorldRequest req, CancellationToken ct)
    {
        var world = await _repository.GetAdventureWorldAsync(req.WorldId, ct);
        await SendAsync(world, cancellation: ct);
    }
}
