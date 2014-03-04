simpletown
==========



### TODO:
- Must:
  - The woodcutter doesn't produce wood or logs
  - The house and The woodcutter should have different colors than the background
  - The trees should check if there isn't a building before "spawning"
  - The trees and buildings aren't blocking the path of the pathfinding
- Want:
  - The workers should be move using double click instead of a single click.
  - The buildings need a "waypoint" where the workers could throw and pickup the resources.
  - The builings should have a list of workers in it.
  - The units (house and buildings) should have a way to be selected and the user can see some info about it.
- Hopefully:
  - When the workers carry a material, it should be shown in the unit.
  - The buttons should be nicer :P

### Bugs:
- Somehow a cube is build at 0,0,0. It shows the progress bar, so it must be a building.
- The units when arrives and changes the new waypoint, they slow down and accelerates again. Very annoying. This is because i'm using the Vecto3.Lerp. I should try with the CharacterController.SimpleMove.
