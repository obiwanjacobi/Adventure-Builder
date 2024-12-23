# Adventure Builder

--- 

## TODO

- [ ] Setup SignalR to notify the web site when new characters enter a passage.
  https://learn.microsoft.com/en-us/dotnet/aspire/real-time/azure-signalr-scenario
- [ ] Setup authentication on Orleans similar to how the api-service works.
- [ ] Activity Log. Lets the player review all the moves that he/she played and events that occured.
- [ ] 

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
