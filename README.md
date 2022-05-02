# Ori DE Mod Loader

Loads any mods in the `Mods` folder at startup. Injected by [MInject](https://github.com/EquiFox/MInject) via the Launcher project.

## Usage for users

Currently the Launcher expects both `0Harmony.dll` and `OriDeModLoader.dll` in the working directory (by default that is the same directory as `Launcher.exe`).

Copy mod assemblies (e.g Mod.dll) into `C:\moon\Ori and the Blind Forest Mod Loader\Mods`.

## Usage for developers

Reference the `OriDeModLoader` nuget package. Any classes that implement `OriDeModLoader.IMod` will be instantiated as a new mod when the game is launched.

This includes stripped versions of game assemblies to build against. They contain definitions but no game code.