# Rogueskiv configuration

## data/config.yaml

### graphics

- **maximized**: game is open maximized
- **screenSize**: only applies if maximized = false
- **maxGameStepsWithoutRender**: game engine parameter to force rendering if execution is slow
- **cameraMovementFriction**: when player moves, the camera takes some time to catch up; the higher this value is, the more it takes to catch up; if = 1, there is no delay

### sound

- **soundsOn**: sound effects enabled
- **musicOn**: music enabled
- **menuMusicFilePath**: music for the menu
- **menuMusicVolume**: from 0 to 100

### game

- **maxFloorCount**: max value for menu floor setting option
- **minFloorCount**: min value for menu floor setting option
- **floorCount**: default value for menu floor setting option
- **gameModeIndex**: default value for menu game mode setting option; indexes correspond to the alphabetic order of the files in `data/gameModes`

## data\gameModes\[game mode name].yaml

### graphics

- **inGameTimeVisible**: visibility of the in-game timer
- **realTimeVisible**: visibility of the real-time timer

### sound

- **gameMusicFilePath**: music for the game
- **gameMusicVolume**: from 0 to 100

### player

- **playerRadius**: player character size
- **playerAcceleration**: acceleration for the player (weird units)
- **playerMaxSpeed**: max player speed in pixels per second (applies to each x-y component independently)
- **playerStopSpeed**: speed in pixels per second at which the player stops (applies to each x-y component independently)
- **initialPlayerHealth**: initial health
- **initialPlayerVisualRange**: initial visual range
- **playerFrictionFactor**: the higher this value, the sooner the player will stop when not pressing any key
- **playerBounceMomentumConservationFactor**: the lower the value, the more the player will slow down when bouncing against a wall; if = 0, the player will stop when touching a wall; if = 1, the player will bounce without loosing speed

### items

- **minFoodSpawnDistanceFactor**: min distance (see Distance factors) from the initial position of the player to spawn food in each floor
- **minTorchSpawnDistanceFactor**: min distance (see Distance factors) from the initial position of the player to spawn torches in each floor
- **minMapRevealerSpawnDistanceFactor**: min distance (see Distance factors) from the initial position of the player to spawn the map revealer in each floor
- **minAmuletSpawnFactor**: min distance (see Distance factors) from the initial position of the player to spawn the amulet in the final floor
- **minDownStairsSpawnFactor**: min distance (see Distance factors) from the initial position of the player to spawn downstairs in each floor
- **maxItemPickingTime**: time in seconds it takes the player to pick up an object
- **foodHealthIncrease**: how much picking food increases health
- **torchVisualRangeIncrease**: how much picking a torch increases visual range

### enemies

- **enemyCollisionDamage**: how much health the player looses when hitting an enemy
- **enemyCollisionBounce**: multiplier to how much the player bounces when colliding with an enemy (the final bounce depends on how close the player really is from the enemy)
- **minSpaceToSpawnEnemy**: how many free tiles must be around each enemy spawn point
- **minEnemySpawnDistance**: min distance in tiles from the initial position of the player to spawn enemies in each floor
- **enemyRadius**: enemies size
- **enemyAnglesProbWeightRanges**: probability weight ranges for the enemy angles (see Weighted ranges)
  - 4 angles = only horizontal / vertical
  - 8 angles =  horizontal, vertical and diagonal
  - 16 angles = 16 possible directions at intervals of 22.5º

### map generation params

- **minRoomSize**: min size for the rooms
- **minRoomSeparation**: min tiles space between rooms
- **mapSizeRange**: range for map sizes in each floor
- **enemyNumberRange**: range for number of enemies in each floor
- **minEnemySpeedRange**: range for the min enemy speed (in pixels per second) for each floor
- **maxEnemySpeedRange**: range for the max enemy speed (in pixels per second) for each floor
- **roomExpandProbRange**: range of probabilities of a room expanding in each loop when generating the map
- **corridorTurnProbRange**: range of probabilities of a corridor making a turn in each loop when generating the map
- **minMapDensityRange**: range of min density for a map to be consider valid
- **initialRoomsRange**: range of initial rooms for the map generation (only those room that reach minRoomSize will be kept)
- **corridorWidthProbWeightRanges**: probability weight ranges for the corridor widths (see Weighted ranges)

## Distance factors

The distances used in a floor are defined based on the distance of the furthest cell. If a specific distance factor is 0.5, that distance, for a specific level, will be (0.5 * distance-of-the-furthest-cell).

## Ranges

Those values dependent on the floor have an `start` and `end` values. When a floor is created, the value used is proportional to the floor factor. For the 1st floor, the `start` value will be used, for the last floor, the `end` value will be used.

There is one exception: if floor count is 1, then the end value is used.

## Weighted ranges

These options allow to control the frequency of some values for different floors. To get a value for this properties, first we calculate the `weight` of each option as described in Ranges. Then we select a random `value` weighting each option by its `weigh`.

If the `value` is < 0 for any given floor, that option will not be used in any case.
