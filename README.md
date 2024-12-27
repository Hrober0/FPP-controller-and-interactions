# FPP player controller

FPP player controller featuring a flexible object interaction system, enabling seamless picking up, moving, and manipulating of objects with realistic physics and intuitive controls.

My code from game name Rat jumple <br>
Game was made on Brackeys Game Jam 2021.2 in week.

Features:
- FPP controller
- Selecting objects
- Lifting, carrying, and throwing items
- Useable tool such as a broom and a mop
- System that allows you to arrange things at zones automatically
- Paths for rats and generic system of their behaviors

Game is available on: [itch.io/hrober/rat-jumble](https://hrober.itch.io/rat-jumble)

Game trailer: [youtube.com/hrober/rat-jumble-trailer](https://www.youtube.com/watch?v=v8mOhGNQn1E)

## Interesting parts

Player movement:<br>
[github.com/Hrober0/FPP-controller-and-interactions/Scripts/Player](https://github.com/Hrober0/FPP-controller-and-interactions/tree/main/Scripts/Player)

Player interactions:<br>
[github.com/Hrober0/FPP-controller-and-interactions/Scripts/PlayerInteractions.cs](https://github.com/Hrober0/FPP-controller-and-interactions/blob/main/Scripts/Player/PlayerInteractions.cs)

One of usable tools:<br>
[github.com/Hrober0/FPP-controller-and-interactions/Scripts/Objects/Tools/Mopp/Mopp.cs](https://github.com/Hrober0/FPP-controller-and-interactions/blob/main/Scripts/Objects/Tools/Mopp/Mopp.cs)

Rat behavior:<br>
[github.com/Hrober0/FPP-controller-and-interactions/Scripts/Mouse/MouseBehavior.cs](https://github.com/Hrober0/FPP-controller-and-interactions/blob/main/Scripts/Mouse/MouseBehavior.cs)

Mess manager - counts points of a mess on map:<br>
[github.com/Hrober0/FPP-controller-and-interactions/Scripts/Master/MessManager.cs](https://github.com/Hrober0/FPP-controller-and-interactions/blob/main/Scripts/Master/MessManager.cs)

UI escape menu:<br>
[github.com/Hrober0/FPP-controller-and-interactions/Scripts/UI/UIEscapeMenu.cs](https://github.com/Hrober0/FPP-controller-and-interactions/blob/main/Scripts/UI/UIEscapeMenu.cs)

## Used Technologies

#### Unity and C#
- UI toolkit
- new input system
#### Additional package:
- NaughtyAttributes (extension for the Unity Inspector): https://github.com/dbrizov/NaughtyAttributes
- DOTween: http://dotween.demigiant.com
