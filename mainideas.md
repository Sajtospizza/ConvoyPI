# Convoy optimizer algorithm features

These are the ideas for the program

## Classes

### Node

#### Properties

- Id (int, should start from 0 with it being the spawn)
- Position (x and y, ints)
- IsTaken (bool)

#### Functions

- TODO

### Car

#### Properties

- Id (int)
- Location (x and y, ints, needs to know if close to node)
- State (Idle, Carrying) - also tells if on node, can only be idle on node

### Factory

#### Properties

- Id (int)
- StartNode and EndNode (Nodes)
- ProcessTime (int, seconds)
- IsProcessing (bool)
- InputQueue length
- OutputQueue length

### Optimizer

Main class, basically the engine, setup and start methods

## Other variables / parameters

- Resource timer
- Incoming resource queue
- Final product queue
- Times and queue lenghts (3 timers and 4 queues)

## Other ideas

- Matrix for nodes next to each other
- Can car reach the position? Direction to go in
- Number of cars should be changeable too!
