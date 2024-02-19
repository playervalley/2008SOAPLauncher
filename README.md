# 2008SOAPLauncher

An addon launcher for Sodikm that allows launching of 2008 Roblox clients (and servers using SOAP).

Built with C# and .NET Framework 3.5.

# Usage

Put the built executable inside of the `Player` directory on your Sodikm client.

If you need to change the client name (e.g. your client is different from `2008E`), you need to rebuild the launcher with the `CurrentPath` variable changed inside of `Program.cs`.

In the `sodikm.ini` of your client, you want to configure it like this:
```
PlayerName=2008SOAPLauncher.exe
PlayerLaunchArgs="{0}"
IsRobloxApp=true
NewSignatures=true
```
NewSignatures is set to true due to these early clients not supporting signatures, so they are commented out.

# Building

You can open the project in Visual Studio 2019 or later and build it that way.

Or, if you don't want to use Visual Studio at all, you can use [MSBuild](https://community.chocolatey.org/packages/visualstudio2019buildtools) to build the program:

```msbuild [Path to your solution(*.sln)] /t:Build /p:Configuration=Release /p:TargetFrameworkVersion=v3.5```
