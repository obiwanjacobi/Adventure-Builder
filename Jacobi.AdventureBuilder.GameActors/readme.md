# Adventure Builder Game Actors

Contains the Orleans Grains implementation for all the game objects.


## Grain Identities

Metadata / Adventure Info objects.

| Info Type | Identity | Description |
| -- | -- | -- |
| WorldInfo | Guid | |
| PassageInfo | WorldId + Int64 | |
| NPC-Info | WorldId + Int64 | |
| CommandInfo | WorldId + Int64 | |

| Grain Type | Identity | Description |
| -- | -- | -- |
| World Instance | New Guid | |
| Passage Instance | WorldId + Int64 | |

## Persistence


