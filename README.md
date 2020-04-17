![CI Status](https://github.com/aevitas/unity-patch/workflows/CI/badge.svg)

Unity Patch
===========

This repository contains a patch for Unity that allows you to set options inaccessible from the application's menus.

Currently, the only supported option for the patch is switching between the dark and light themes in Unity.

Usage
=====

We provide binaries for Windows 10, Linux, and macOS. All compiled binaries are x64.
See the [release section](https://github.com/aevitas/unity-patch/releases).
Alternatively, you can build the patch from source.

Run `patcher.exe` on windows, or alternatively `Patcher` on Linux or MacOS. By default, it will locate your Unity install
at `C:\Program Files\Unity\Editor\Unity.exe`, which is obviously wrong for both Linux and macOS, and it will set your theme to dark.

You can pass various arguments to the patcher:

* `exe=` or `e=` to specify the location of the Unity executable
* `theme=` or `t=` to set the theme, currently only `light` or `dark` are valid
* `help` or `h` to display the options the patcher supports
* `--windows` for Windows builds of Unity
* `--linux` for Linux builds of Unity
* `--mac` for MacOS builds of Unity
* `--force` or `--f` to gently apply force

Depending on your system, looking up the offsets to patch can take a couple moments.

Unity Versions
--------------

The patcher supports multiple versions of Unity. Versions can be specified by passing the `-v=` or `--version=` command line argument.

For instance, if you want to patch Unity version 2020.1 on Windows, you'd run:

```
patcher.exe --windows --version=2020.1 --t=dark
```

Currently, the following OS and Unity version combinations are supported:

|        | Windows            | MacOS              | Linux              |
|--------|:------------------:|:------------------:|:------------------:|
| 2020.1 | :white_check_mark: | :white_check_mark: |         :x:        |
| 2019.3 | :white_check_mark: | :white_check_mark: |         :x:        |
| 2019.2 | :white_check_mark: |         :x:        | :white_check_mark: |
| 2019.1 |         :x:        | :white_check_mark: |         :x:        | 
| 2018.4 | :white_check_mark: | :white_check_mark: |         :x:        |
| 2018.3 | :white_check_mark: |         :x:        |         :x:        |
| 2018.2 | :white_check_mark: |         :x:        |         :x:        |

If you don't specify a version, the patcher will choose a default version.

Troubleshooting
===============

To get the highest chance of success, you should always run the patch on a clean install of Unity. If that doesn't work, you can try:

* Resetting your user preferences either manually or by calling [`EditorPrefs.DeleteAll()`](https://github.com/aevitas/unity-patch/issues/17#issuecomment-592070343)

Issues
======

If the patcher doesn't work, please let us know by opening an [issue](https://github.com/aevitas/unity-patch/issues), and provide as much details as you can. We provide some issue templates for common issues - please use them when applicable. They help us resolve issues faster.

Linux and MacOS
===============

When running the patcher on Linux or MacOS, be sure to run the respective binaries for your operating system. They are located in `osx-x64` for Mac, and `linux-x64` for Linux.

* Mac users should run the patcher with the `--mac` command line option
* Linux users should run the patcher with the `--linux` command line option

For example, on Linux you would run:

`sudo ./linux-x64/Patcher -e=/path/to/Unity --t=dark --linux`

or on Mac:

`sudo ./osx-64/Patcher -e=/Applications/Unity/Hub/Editor/<VERSION>/Unity.app/Contents/MacOS/Unity --mac --t=dark`

**NOTE:**
On MacOS Unity might be displaying a mix of Dark and Light Themes after patching. This can be resolved by restarting Unity. After restarting Unity the Theme should display correctly.

If you get `command not found`, try changing permissions for the file by running `chmod +x Patcher`. If running the patcher again gives you the following error `Can not patch the specified file - it is marked as read only!` then you need to check Unity to ensure you have write permissions for the `Unity.app` file as well.
