using FastEndpoints;
using FluentValidation;
using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.ApiService.Adventure;

internal sealed record GetAdventureNonPlayerCharacterRequest(string WorldId, long NpcId);

internal sealed class GetAdventureNonPlayerCharacterValidator : Validator<GetAdventureNonPlayerCharacterRequest>
{
    public GetAdventureNonPlayerCharacterValidator()
    {
        RuleFor(r => r.WorldId)
            .NotEmpty();
        RuleFor(r => r.NpcId)
            .NotEmpty();
    }
}

internal sealed class GetAdventureNonPlayerCharacterEndpoint : Endpoint<GetAdventureNonPlayerCharacterRequest, AdventureNonPlayerCharacterInfo>
{
    private readonly IAdventureRepository _repository;

    public GetAdventureNonPlayerCharacterEndpoint(IAdventureRepository repository)
        => _repository = repository;

    public override void Configure()
    {
        Get("adventure/worlds/{WorldId}/npcs/{NpcId}");
        // TODO: authentication
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAdventureNonPlayerCharacterRequest req, CancellationToken ct)
    {
        var worldData = await _repository.GetAdventureWorldAsync(req.WorldId, ct);
        var npc = AdventureMapper.ToNonPlayerCharacterInfo(worldData, req.NpcId);
        await SendAsync(npc, cancellation: ct);
    }
}
