using FastEndpoints;
using FluentValidation;
using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.ApiService.Adventure;

internal sealed record GetAdventureWorldRequest(string WorldId);

internal sealed class GetAdventureWorldValidator : Validator<GetAdventureWorldRequest>
{
    public GetAdventureWorldValidator()
    {
        RuleFor(r => r.WorldId)
            .NotEmpty();
    }
}

internal sealed class GetAdventureWorldEndpoint : Endpoint<GetAdventureWorldRequest, AdventureWorldInfo>
{
    private readonly IAdventureRepository _repository;

    public GetAdventureWorldEndpoint(IAdventureRepository repository)
        => _repository = repository;

    public override void Configure()
    {
        Get("adventure/worlds/{WorldId}");
        // TODO: authentication
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAdventureWorldRequest req, CancellationToken ct)
    {
        var worldData = await _repository.GetAdventureWorldAsync(req.WorldId, ct);
        var world = AdventureMapper.ToWorldInfo(worldData);
        await SendAsync(world, cancellation: ct);
    }
}
