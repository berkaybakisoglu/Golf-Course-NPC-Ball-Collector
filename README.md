1) Map Environment
Areas:
The map contains four types of terrain layers: Grass, Light Grass, Sand, and a Corrupted Area.
Effects on NPC:
•	Grass, Light Grass: 1 cost to navmesh; original NPC speed.
•	Sand: 2 cost to navmesh; speed decreases to half.
•	Corrupted Area: 3 cost to navmesh; speed decreases to a quarter.
Assumptions:
•	There will be one NPC and one golf cart (sorry but its a bus currently).
•	Water is not walkable.
•	Some areas allow NPCs to jump using navmesh links.
•	There are no steep slopes requiring animation IK polishing.
•	Golf balls are static and do not move on their own.
2) Character Movement
•	Player has no direct effect on the NPC character.
•	The NPC uses navmesh for navigation and can walk, run, collect, score, and jump.
3) Golf Balls
•	Current Setup: 3 golf balls are randomly created at the start of the game.
Ball Creation Assumptions:
•	Balls are always created on flat terrain.
•	They must be reachable by the player.
Level Decision:
The game selects a random valid spot for each golf ball on the map.Currently 30 balls created,randomly on every start.
Spot Selection:
o	Calculate the path cost between the spot and the scoring area.
o	Normalize the value with the maximum path.
Threshold Evaluation:
o	Easy: Close to the scoring area with no obstacles (0-0.2 or no obstacle).
o	Medium: Slightly farther or near obstacles (0.2-0.6 or obstacle close enough).
o	Hard: Far away or requires tricky paths like linked jumps (0.6+ or jump-needed paths).
Scoring Levels:
•	Level 1: White ball, 1 point.
•	Level 2: Yellow ball, 2 points.
•	Level 3: Red ball, 3 points.
4) Health Mechanic
•	Health decreases over time with a configurable offset.
•	Default settings: Starts at 100 and decreases at a weight of 0.5, making the game last approximately 2 minutes before a game over.

5) Decision Making
The NPC selects the most desirable golf ball based on the following formula:
Score = Value of the Ball – Distance Cost – Proximity Cost
Value of the Ball:
o	Higher-value balls are more desirable.
o	If the NPC is healthy, it prioritizes high-value balls even more.
Distance Cost:
o	Farther balls require more effort.
o	NPC with low health prefers closer balls to save energy.
Proximity Cost:
o	Balls closer to the scoring zone are easier to score once collected.
These all have some weights and can be change on editor.
Simplified Approach:
•	Since the player starts near the scoring zone, the NPC can initially discard distance costs and formula can be simplified.However i kept it in any case if we collect more than one ball once.
6)Extra
Added a minimap to see what’s going on on player’s around , and also added a free cam which can activate and disabled by f button, on the cam mode mouse and asdweq can change the rotation and position. (Makes easy to debug , kept them)

