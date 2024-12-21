# Adventure Builder

Web - Player Frontend
API - manages game meta data
GameServer - Running game state

## Start a new Game

The Player selects one of the games and starts it up.

Web -> API

Web - Game Library - Select Game - Show game details - press Start New

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
