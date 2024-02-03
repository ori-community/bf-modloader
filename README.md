# Ori DE Mod Loader

This repo has been archived - please use [ori-modding-bf-core](https://github.com/ori-community/ori-modding-bf-core) instead.

Loads mods with [MInject](https://github.com/EquiFox/MInject) or [Doorstop](https://github.com/NeighTools/UnityDoorstop)

## Usage for users

Just use the [Mod Manager](https://github.com/Kirefel/bf-mod-manager)

TODO make manual installation a bit nicer

## Usage for developers

Reference the `OriDeModLoader` nuget package. Any classes that implement `OriDeModLoader.IMod` will be instantiated as a new mod when the game is launched.

This includes stripped versions of game assemblies to build against. They contain definitions but no game code.
