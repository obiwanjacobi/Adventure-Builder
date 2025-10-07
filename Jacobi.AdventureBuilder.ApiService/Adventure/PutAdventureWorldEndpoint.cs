using FastEndpoints;
using FluentValidation;
using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.ApiService.Adventure;

internal sealed class PutAdventureWorldValidator : Validator<AdventureWorldInfo>
{
    public PutAdventureWorldValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty();
        RuleFor(r => r.Name)
            .NotEmpty();
    }
}

internal sealed class PutAdventureWorldEndpoint : Endpoint<AdventureWorldInfo, AdventureWorldInfo>
{
    private readonly IAdventureRepository _repository;

    public PutAdventureWorldEndpoint(IAdventureRepository repository)
        => _repository = repository;

    public override void Configure()
    {
        Put("adventure/worlds/{WorldId}");
        // TODO: authentication
        AllowAnonymous();
    }

    public override async Task HandleAsync(AdventureWorldInfo req, CancellationToken ct)
    {
        var worldData = AdventureMapper.ToWorldData(req);
        await _repository.PutAdventureWorldAsync(worldData, ct);
        await Send.OkAsync();
    }
}
