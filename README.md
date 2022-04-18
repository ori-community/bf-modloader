# Ori DE Mod Loader

Loads any mods in the `Mods` folder at startup. Injected by [Unity Doorstop 3.x](https://github.com/NeighTools/UnityDoorstop).

## Usage for users

Copy package contents to the game's root directory e.g. `steam\steamapps\common\Ori DE`

Copy mod assemblies (e.g. `Mod.dll`) to `Ori DE\Mods`

## Usage for developers

Reference the `OriDeModLoader` nuget package. Any classes that implement `OriDeModLoader.IMod` will be instantiated as a new mod when the game is launched.

This includes stripped versions of game assemblies to build against. They contain definitions but no game code.
