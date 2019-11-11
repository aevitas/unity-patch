Unity Patch
===========

This repository contains a patch for Unity that allows you to set options inacessible from the application's menus.

Currently, the only supported option for the patch is switching between the dark and light themes in Unity.

Usage
=====

We provide binaries for Windows 10, Linux, and macOSX. All compiled binaries are x64. See the [release section](https://github.com/aevitas/unity-patch/releases). Alternatively, you can build the patch from source.

Run `patcher.exe` on windows, or alternatively `Patcher` on Linux or MacOS. By default, it will locate your Unity install at `C:\Program Files\Unity\Editor\Unity.exe`, which is obviously wrong for both Linux and macOS, and it will set your theme to dark.

You can pass various arguments to the patcher:

* `exe=|e=` to specify the location of the Unity executable
* `theme=|t=` to set the theme, currently only `light` or `dark` are valid
* `help|h` to display the options the patcher supports

Depending on your system, looking up the offsets to patch can take a couple moments.

Linux and MacOS
===============

When running the patcher on Linux or MacOS, be sure to run the respective binaries for your operating system. They are located in `osx-x64` for Mac, and `linux-x64` for Linux.

* Mac users should run the patcher with the `-mac` command line option
* Linux users should run the patcher with the `-linux` command line option

For example, on Linux you would run:

`sudo ./linux-x64/Patcher -e=/path/to/Unity -t=dark -linux`
