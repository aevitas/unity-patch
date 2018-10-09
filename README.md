Unity Patch
===========

This repository contains a patch for Unity that allows you to set options inacessible from the application's menus.

Currently, the only supported option for the patch is switching between the dark and light themes in Unity.

Usage
=====

We provide binaries for Windows 10, Linux, and macOSX. All compiled binaries are x64. See the [release section](https://github.com/aevitas/unity-patch/releases). Alternatively, you can build the patch from source.

Run `patcher.exe` on windows, `patcher.dll` on Linux. By default, it will locate your Unity install at `C:\Program Files\Unity\Editor\Unity.exe`, which is obviously wrong for both Linux and macOS, and it will set your theme to dark.

You can pass various arguments to the patcher:

* `exe=|e=` to specify the location of the Unity executable
* `theme=|t=` to set the theme, currently only `light` or `dark` are valid

Depending on your system, looking up the offsets to patch can take a couple moments.
