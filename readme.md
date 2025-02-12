# Adventure Builder

---

## TODO

- [ ] Setup authentication on Orleans similar to how the api-service works.
- [ ] Asset and NPC commands provided by grain itself?
  Tags that identify what common commands can be used on the asset/npc?
- [ ] Display asset and npc commands inside passage log line for the given asset/npc (not in commandbar)
- [x] Taking and dropping assets repeately does not generate new (sub)log lines.
- [ ] => Also not for the other players that are notified of the take/drop.
- [ ] Notify other players when an asset is taken (player x took the 'asset')
- [ ] Add inter-player chat.
- [ ] Talk to npc's.
- [ ] Count how many times a player visits a passage. Other rules may use this info.
- [ ] A concept of public and private inventory items? That way you can see a player has the bottle and ask them for it.
- [ ] Lock assets when they are picked up - so 2 players cannot pick up the same asset at the same time.
- [ ] What happens to assets in a player's inventory when he/she leaves the game?
- [ ] Certain assets introduce new commands (to be done with the asset)
  Bottle->drink(if-open),break,close,open, Knife->cut,stab,slice, 
  How to manage the dependency between asset defined in twine and the code for its commands?
  How to manage the state an asset can be in (bottle: open,closed,empty,notempty,broken)/StateTable.
- [ ] Refactor GrainState to use injected state https://learn.microsoft.com/en-us/dotnet/orleans/grains/grain-persistence/?pivots=orleans-7-0

## Done

- [x] Setup SignalR to notify the web site when new characters enter a passage.
  https://learn.microsoft.com/en-us/dotnet/aspire/real-time/azure-signalr-scenario
- [x] Activity Log. Lets the player review all the moves that he/she played and events that occured.
- [x] Command handlers / providers
- [x] Player inventory.

---

## Docker Desktop

For development Docker Desktop is required.
It runs the Azure CosmosDB emulator and the Azure Storage emulator (Azurite).
It also runs a container for Keycloak (identity).

---

## Azure Storage Explorer

To connect Azure Storage Explorer (Desktop App) to the Storage Account running in the container do the following:

In Aspire Dashboard:
- Navigate to the Storage Account Container resource in the dashboard
- Open it's details
- Go to the Endpoints sections and note the 'target ports' for blobs, queues and tables.

In Azure Storage Explorer:
- Right-click on the 'Storage Accounts' in the Explorer tree and choose 'Connect to Azure Storage'
- Select the 'Local storage emulator' option at the bottom
- Replace the default port values with the 'target port' values from the Aspire dashboard
- Create the connection.

--

## Orleans SDK and Storage-related Packages verion

> Keep the Orleans SDK and the Orleans Azure Storage related packages on version `8.2.0`!

The Azurite Azure Storage Emulator will generate an error when you upgrade.

```txt
Azure.RequestFailedException
  HResult=0x80131500
  Message=The API version 2025-01-05 is not supported by Azurite. Please upgrade Azurite to latest version and retry. If you are using Azurite in Visual Studio, please check you have installed latest Visual Studio patch. Azurite command line parameter "--skipApiVersionCheck" or Visual Studio Code configuration "Skip Api Version Check" can skip this error. 
RequestId:3048fea1-58e7-4110-b79d-316befcaf36b
Time:2025-01-22T13:16:47.602Z
Status: 400 (The API version 2025-01-05 is not supported by Azurite. Please upgrade Azurite to latest version and retry. If you are using Azurite in Visual Studio, please check you have installed latest Visual Studio patch. Azurite command line parameter "--skipApiVersionCheck" or Visual Studio Code configuration "Skip Api Version Check" can skip this error. )
ErrorCode: InvalidHeaderValue

Content:
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Error>
  <Code>InvalidHeaderValue</Code>
  <Message>The API version 2025-01-05 is not supported by Azurite. Please upgrade Azurite to latest version and retry. If you are using Azurite in Visual Studio, please check you have installed latest Visual Studio patch. Azurite command line parameter "--skipApiVersionCheck" or Visual Studio Code configuration "Skip Api Version Check" can skip this error. 
RequestId:3048fea1-58e7-4110-b79d-316befcaf36b
Time:2025-01-22T13:16:47.602Z</Message>
</Error>
```
