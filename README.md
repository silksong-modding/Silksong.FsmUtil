# Silksong.FsmUtil

This is a library for mods to make FSM edits easier.

## Installation

To add Silksong.FsmUtil to your mod, add the following line to your .csproj:
```
<PackageReference Include="Silksong.FsmUtil" Version="0.3.5" />
```
The most up to date version number can be retrieved from [Nuget](https://www.nuget.org/packages/Silksong.FsmUtil).

You will also need to add a dependency to your thunderstore.toml:
```
silksong_modding-FsmUtil = "0.3.5"
```
The version number does not matter hugely, but the most up to date number can be retrieved from
[Thunderstore](https://thunderstore.io/c/hollow-knight-silksong/p/silksong_modding/FsmUtil/).
If manually uploading, instead copy the dependency string from the Thunderstore link.

It is recommended to add FsmUtil as a BepInEx dependency by putting the following attribute
onto your plugin class, below the BepInAutoPlugin attribute.
```
[BepInDependency("org.silksong-modding.fsmutil")]
```

# EUPL
                      Copyright (c) 2025 silksong-modding
                      Licensed under the EUPL-1.2
https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12
