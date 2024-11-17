# Game Design Overview

## Map Environment Areas
The map contains four types of terrain layers:(Speed debuffs can change by editor)

- **Grass, Light Grass**: 
  - **Navigation Cost**: `1`
  - **NPC Speed**: Original speed
- **Sand**: 
  - **Navigation Cost**: `2`
  - **NPC Speed**: Half of the original speed
- **Corrupted Area**: 
  - **Navigation Cost**: `3`
  - **NPC Speed**: A quarter of the original speed

### Assumptions
- There will be one NPC and one golf cart (currently a bus).
- Water is not walkable.
- Some areas allow NPCs to jump using navmesh links.
- No steep slopes requiring animation IK polishing.
- Golf balls are static and do not move on their own.

## Character Movement
- The player has no direct effect on the NPC character.
- The NPC uses navmesh for navigation and can:
  - Walk
  - Run
  - Collect
  - Score
  - Jump (Cost 2 on navmesh)

## Golf Balls
### Current Setup
- 3 golf balls are randomly created at the start of the game.
- **Ball Creation Assumptions**:
  - Balls are always created on flat terrain.
  - Balls must be reachable by the player.

### Level Decision
The game selects a random valid spot for each golf ball on the map.

#### Spot Selection
1. Calculate the path cost between the spot and the scoring area.
2. Normalize the value with the maximum path.

#### Threshold Evaluation
- **Easy**: Close to the scoring area with no obstacles (`0-0.2` or no obstacle).
- **Medium**: Slightly farther or near obstacles (`0.2-0.6` or obstacle close enough).
- **Hard**: Far away or requires tricky paths like linked jumps (`0.6+` or jump-needed paths).

#### Scoring Levels
- **Level 1**: White ball, `1` point.
- **Level 2**: Yellow ball, `2` points.
- **Level 3**: Red ball, `3` points.

## Health Mechanic
- Health decreases over time with a configurable offset.
- **Default Settings**: 
  - Starts at `100`
  - Decreases at a weight of `0.5`
  - Game lasts approximately `3 minutes` before game over.

## Decision Making
The NPC selects the most desirable golf ball based on the following formula:
Score = Value of the Ball – Distance Cost – Proximity Cost

### Factors
- **Value of the Ball**:
  - Higher-value balls are more desirable.
  - If the NPC is healthy, it prioritizes high-value balls even more.
- **Distance Cost**:
  - Farther balls require more effort.
  - NPC with low health prefers closer balls to save energy.
- **Proximity Cost**:
  - Balls closer to the scoring zone are easier to score once collected.

All weights are adjustable in the editor.

### Simplified Approach
- Since the player starts near the scoring zone, the NPC can initially discard distance costs.
- The formula remains in place in case multiple balls are collected at once.

## Additional Features
1. **Minimap**:(Makes gameplay better and useful for debugging.)
   - Displays the area around the player. 
2. **Free Cam**: (Mostly for debugging to see what's going on the map.)
   - Activated/Deactivated using the `F` button.
   - Camera movement: `ASDWEQ` for position and rotation.


