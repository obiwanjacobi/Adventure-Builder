using FastEndpoints;
using FluentValidation;
using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.ApiService.Adventure;

internal sealed record GetAdventurePassageRequest(string WorldId, long PassageId);

internal sealed class GetAdventurePassageValidator : Validator<GetAdventurePassageRequest>
{
    public GetAdventurePassageValidator()
    {
        RuleFor(r => r.WorldId)
            .NotEmpty();
        RuleFor(r => r.PassageId)
            .NotEmpty();
    }
}

internal sealed class GetAdventurePassageEndpoint : Endpoint<GetAdventurePassageRequest, AdventurePassageInfo>
{
    private readonly IAdventureRepository _repository;

    public GetAdventurePassageEndpoint(IAdventureRepository repository)
        => _repository = repository;

    public override void Configure()
    {
        Get("adventure/worlds/{WorldId}/passages/{PassageId}");
        // TODO: authentication
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAdventurePassageRequest req, CancellationToken ct)
    {
        var worldData = await _repository.GetAdventureWorldAsync(req.WorldId, ct);
        var passage = AdventureMapper.ToPassageInfo(worldData, req.PassageId);
        await SendAsync(passage, cancellation: ct);
    }
}
