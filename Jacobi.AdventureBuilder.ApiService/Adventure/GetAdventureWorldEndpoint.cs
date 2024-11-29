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

    private static AdventureWorldInfo CreateTestWorld(string adventureId)
    {
        return new AdventureWorldInfo()
        {
            Id = adventureId,
            Name = $"Adventure {adventureId}",
            Passages = [
                new AdventurePassageInfo()
                {
                    Id = 1,
                    Name = "Room 1",
                    Description = "You are in Room 1",
                    Commands = [
                        new AdventureCommandInfo
                        {
                            Id = "nav-north",
                            Kind = "nav-room",
                            Name = "North",
                            Description = "To the north there is a door leading to another room.",
                            Action = "nav:room:2"
                        }
                    ]
                },
                new AdventurePassageInfo()
                {
                    Id = 2,
                    Name = "Room 2",
                    Description = "You are in Room 2",
                    Commands = [
                        new AdventureCommandInfo
                        {
                            Id = "nav-south",
                            Kind = "nav-room",
                            Name = "South",
                            Description = "To the south there is a door leading to another room.",
                            Action = "nav:room:1"
                        }
                    ]
                }
            ],
            NonPlayerCharacters = []
        };
    }
}
