using Jacobi.AdventureBuilder.AdventureModel;

namespace Jacobi.AdventureBuilder.Twine;

internal sealed class AdventureModelBuilder
{
    private readonly List<AdventureRoomInfo> _rooms = [];

    public void AddRoom(long id, string name, string description, IReadOnlyCollection<AdventureCommandInfo> commands)
    {
        var room = new AdventureRoomInfo()
        {
            Id = id,
            Name = name,
            Description = description,
            Commands = commands,
        };

        _rooms.Add(room);
    }

    public static AdventureCommandInfo CreateNavigateCommand(string id, string name, string description)
    {
        return new AdventureCommandInfo
        {
            Id = id,
            Name = name,
            Description = description,
            Kind = "nav-room",
            Action = $"nav:room:{id}",
        };
    }

    public AdventureWorldInfo Build(string id, string name)
    {
        var world = new AdventureWorldInfo
        {
            Id = id,
            Name = name,
            Rooms = _rooms.ToList()
        };

        _rooms.Clear();

        return world;
    }
}
