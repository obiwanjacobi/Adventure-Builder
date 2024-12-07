using FastEndpoints;
using FluentValidation;
using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.ApiService.Adventure;

internal sealed record GetAdventureAssetRequest(string WorldId, long AssetId);

internal sealed class GetAdventureAssetValidator : Validator<GetAdventureAssetRequest>
{
    public GetAdventureAssetValidator()
    {
        RuleFor(r => r.WorldId)
            .NotEmpty();
        RuleFor(r => r.AssetId)
            .NotEmpty();
    }
}

internal sealed class GetAdventureAssetEndpoint : Endpoint<GetAdventureAssetRequest, AdventureAssetInfo>
{
    private readonly IAdventureRepository _repository;

    public GetAdventureAssetEndpoint(IAdventureRepository repository)
        => _repository = repository;

    public override void Configure()
    {
        Get("adventure/worlds/{WorldId}/assets/{AssetId}");
        // TODO: authentication
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAdventureAssetRequest req, CancellationToken ct)
    {
        var worldData = await _repository.GetAdventureWorldAsync(req.WorldId, ct);
        var npc = AdventureMapper.ToAssetInfo(worldData, req.AssetId);
        await SendAsync(npc, cancellation: ct);
    }
}
