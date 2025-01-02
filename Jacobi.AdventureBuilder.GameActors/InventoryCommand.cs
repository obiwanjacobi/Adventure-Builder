using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public class InventoryCommandHandler : IGameCommandHandler
{
    public const string PutInventory = "inv-put";
    public const string TakeInventory = "inv-take";

    private readonly IGrainFactory _factory;

    public InventoryCommandHandler(IGrainFactory factory)
    {
        _factory = factory;
    }

    public bool CanHandleCommand(GameCommand command)
    {
        return command.Kind == PutInventory || command.Kind == TakeInventory;
    }

    public async Task<GameCommandResult> HandleCommand(GameCommandContext context, GameCommand command)
    {
        if (context.Player is null) return new GameCommandResult();

        var cmdAction = GameCommandAction.Parse(command.Action);
        var worldKey = WorldKey.Parse(context.World.GetPrimaryKeyString());
        var assetKey = new AssetKey(worldKey, cmdAction.Id);
        var asset = _factory.GetGrain<IAssetGrain>(assetKey);

        var inventory = await context.Player.Inventory();
        await inventory.Add(asset);
        await context.Passage.Exit(assetKey);

        // TODO
        return new GameCommandResult(context.Passage);
    }

    public async Task<IReadOnlyList<GameCommand>> ProvideCommands(GameCommandContext context)
    {
        var occupants = await context.Passage.Occupants();
        var assetKeys = occupants.Where(occupant => AssetKey.IsValidKey(occupant));
        var assets = assetKeys.Map(assetKey => _factory.GetGrain<IAssetGrain>(assetKey));

        var commands = new List<GameCommand>();
        foreach (var asset in assets)
        {
            var assetName = await asset.Name();
            var cmd = new GameCommand(
                PutInventory,
                $"Take {assetName}",
                $"Add {assetName} to your Inventory.",
                new GameCommandAction(PutInventory, AssetKey.Parse(asset.GetPrimaryKeyString()).AssetId).ToString()
            );
            commands.Add(cmd);
        }

        if (context.Player is not null)
        {
            var inventory = await context.Player.Inventory();
            var items = await inventory.Assets();

            foreach (var item in items)
            {
                var assetName = await item.Name();
                var cmd = new GameCommand(
                    TakeInventory,
                    $"Drop {assetName}",
                    $"Remove {assetName} from your Inventory (leave it here).",
                    new GameCommandAction(TakeInventory, AssetKey.Parse(item.GetPrimaryKeyString()).AssetId).ToString()
                );
                commands.Add(cmd);
            }
        }

        return commands;
    }
}
