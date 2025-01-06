using Jacobi.AdventureBuilder.GameContracts;

namespace Jacobi.AdventureBuilder.GameActors;

public class InventoryCommandHandler : IGameCommandHandler
{
    public const string InventoryPut = "inv-put";
    public const string InventoryTake = "inv-take";

    private readonly IGrainFactory _factory;

    public InventoryCommandHandler(IGrainFactory factory)
    {
        _factory = factory;
    }

    public bool CanHandleCommand(GameCommand command)
    {
        return command.Kind == InventoryPut || command.Kind == InventoryTake;
    }

    public async Task<GameCommandResult> HandleCommand(GameCommandContext context, GameCommand command)
    {
        if (context.Player is null) return new GameCommandResult();

        var cmdAction = GameCommandAction.Parse(command.Action);
        var worldKey = WorldKey.Parse(context.World.GetPrimaryKeyString());
        var assetKey = new AssetKey(worldKey, cmdAction.Id);
        var asset = _factory.GetGrain<IAssetGrain>(assetKey);
        var inventory = await context.Player.Inventory();

        if (command.Kind == InventoryPut)
        {
            await inventory.Add(asset);
            await context.Passage.Exit(assetKey);
        }
        if (command.Kind == InventoryTake)
        {
            await inventory.Remove(asset);
            await context.Passage.Enter(assetKey);
        }

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
            // does asset support inventory-put command?
            var commandIds = await asset.CommandIds();
            if (!commandIds.Exists(cmdId => cmdId == InventoryPut)) continue;

            var assetName = await asset.Name();
            var cmd = new GameCommand(
                InventoryPut,
                $"Take {assetName}",
                $"Add {assetName} to your Inventory.",
                new GameCommandAction(InventoryPut, AssetKey.Parse(asset.GetPrimaryKeyString()).AssetId).ToString(),
                $"Added {assetName} to your inventory.",
                asset.GetPrimaryKeyString()
            );
            commands.Add(cmd);
        }

        if (context.Player is not null)
        {
            var inventory = await context.Player.Inventory();
            var items = await inventory.Assets();

            foreach (var item in items)
            {
                var itemKey = item.GetPrimaryKeyString();
                var assetName = await item.Name();
                var cmd = new GameCommand(
                    InventoryTake,
                    $"Drop {assetName}",
                    $"Remove {assetName} from your Inventory (leave it here).",
                    new GameCommandAction(InventoryTake, AssetKey.Parse(itemKey).AssetId).ToString(),
                    $"Dropped {assetName} here.",
                    itemKey
                );
                commands.Add(cmd);
            }
        }

        return commands;
    }
}
