# Ori DE Mod Loader

Loads any mods in the `Mods` folder at startup. Injected by [MInject](https://github.com/EquiFox/MInject) via the Launcher project.

## Usage for users

Extract anywhere.

Install mods by adding them to the `Mods` folder.

Run `Injector.exe` to launch the game with mods.

## Usage for developers

Reference the `OriDeModLoader` nuget package. Any classes that implement `OriDeModLoader.IMod` will be instantiated as a new mod when the game is launched.

This includes stripped versions of game assemblies to build against. They contain definitions but no game code.
