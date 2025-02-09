# Adventure Builder Game Actors

Contains the Orleans Grains implementation for all the game objects.


## Grain Identities (Key)

Each grain has an identity or primary key. The game objects have composite keys based on the World-instance (key) and their own identification, except for the player grain. Related but different grains can share the same key value. For instance PlayerGrain, PLayerInventoryGrain and PlayerLogGrain all have the same key (but are different types).


## Grain Relations

- World (instance)
  - Passage
    - Commands (navigation)
    - Occupants (Player, NPC or Asset)
      - [Player]
        - Inventory
          - Assets
            - Commands  (Assets in Inventory)
        - Commands
      - [NPC]
        - Commands
      - [Asset]
        - Commands (Assets in Passage)
  - Player
    - PlayerLog (history)
    - PlayerInventory
