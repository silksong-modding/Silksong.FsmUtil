# About Silksong.FsmUtil

[![NuGet Version](https://img.shields.io/nuget/v/Silksong.FsmUtil)](https://www.nuget.org/packages/Silksong.FsmUtil)
[![GitHub License](https://img.shields.io/github/license/silksong-modding/Silksong.FsmUtil?logo=github)](https://github.com/silksong-modding/Silksong.FsmUtil)

Silksong.FsmUtil is a library for interacting with PlayMakerFSMs in code.

With this, it's easy to:
- Use a bunch of utility functions regarding PlayMakerFSMs

## Installation

To add Silksong.FsmUtil to your mod, add the following line to your .csproj:
```
<PackageReference Include="Silksong.FsmUtil" Version="0.3.7" />
```
The most up to date version number can be retrieved from [Nuget](https://www.nuget.org/packages/Silksong.FsmUtil).

You will also need to add a dependency to your thunderstore.toml:
```
silksong_modding-FsmUtil = "0.3.7"
```
The version number does not matter hugely, but the most up to date number can be retrieved from
[Thunderstore](https://thunderstore.io/c/hollow-knight-silksong/p/silksong_modding/FsmUtil/).
If manually uploading, instead copy the dependency string from the Thunderstore link.

It is recommended to add FsmUtil as a BepInEx dependency by putting the following attribute
onto your plugin class, below the BepInAutoPlugin attribute.
```
[BepInDependency("org.silksong-modding.fsmutil")]
```
