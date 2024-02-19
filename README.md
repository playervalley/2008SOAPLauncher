# 2008SOAPLauncher

An addon launcher for Sodikm that allows launching of 2008 Roblox clients (and servers using SOAP).

# Usage

Put the built executable inside of a folder in `data\clients\2008E\Player` on the root of your Sodikm directory. If you need to change the client name, you need to rebuild the launcher with the `CurrentPath` variable changed inside of `Program.cs`.

In `sodikm.ini`, you want to configure it like this:
```
PlayerName=2008SOAPLauncher.exe
PlayerLaunchArgs="{0}"
IsRobloxApp=true
NewSignatures=true
```
NewSignatures is set to true due to these early clients not supporting signatures, so we comment them out so we don't get errors.

# Building

You can open the project in Visual Studio 2019 or later and build it that way.

Or, if you don't want to use Visual Studio at all, [you can use MSBuild](https://docs.revenera.com/installshield25helplib/helplibrary/MSBuild_CmdLine.htm) to build the program.
