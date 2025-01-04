# Adventure Builder

--- 

## TODO

- [ ] Setup authentication on Orleans similar to how the api-service works.
- [ ] Asset and NPC commands provided by grain itself?
  Tags that identify what common commands can be used on the asset/npc?
- [ ] Display asset and npc commands inside passage log line for the given asset/npc (not in commandbar)
- [ ] Taking and dropping assets repeately does not generate new (sub)log lines.
  Also not for the other players that are notified of the take/drop.
- [ ] Notify other players when an asset is taken (player x took the 'asset')
- [ ] Add inter-player chat.
- [ ] Communicate with npc's.
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
