Introduction
============

SimpleVersion is available as a cross-platform dotnet tool and core libraries for direct consumption in a project.

To get a better understanding of the values returned from invoking SimpleVersion
see the [results documentation][results].

Dotnet Tool
-----------------

The latest version of the dotnet tool can be found on nuget.org.
You can install `SimpleVersion.Tool` the [dotnet cli]:

```posh
# Installs the latest version as a global tool
dotnet install -g SimpleVersion.Tool
```

Once installed, you can invoke SimpleVersion in your favourite shell, optionally passing the path to your repository (defaults to current working directory)

```posh
PS c:\MyRepo> simpleversion
PS c:\> simpleversion ./MyRepo
```

Usage in Cake
-------------

The dotnet tool can be integrated into a Cake build script. This makes use
of the [Cake.DotNetTool.Module] to enable installing `SimpleVersion.Tool`.
> You will need to run `.\build.ps1 --bootstrap` prior to running `.\build.ps1` to ensure correct installation of the tool.

A simple `build.cake` can invoke the `SimpleVersion`:

```c#
// Install dotnet core tool module
#module nuget:?package=Cake.DotNetTool.Module&version=0.4.0
// Install SimpleVersion dotnet tool
#tool dotnet:?package=SimpleVersion.Tool&version=0.3.0
// Install newtonsoft to parse result
#addin nuget:?package=Newtonsoft.Json&version=12.0.3
// Import newtonsoft namespace for parsing
using Newtonsoft.Json.Linq;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var versionFormat = Argument("versionFormat", "Semver1");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Build")
    .Does(() =>
{
    // Call SimpleVersion to get version information
    StartProcess(
        Context.Tools.Resolve("simpleversion.exe"),
        new ProcessSettings{ RedirectStandardOutput = true }, 
        out var simpleVersionOut // capture output for parsing
    );
    
    // Parse json result
    dynamic simpleVersion = JObject.Parse(string.Join("", simpleVersionOut));

    // Display version - Access formats dictionary or other properties
    Information(simpleVersion.Formats[versionFormat]);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
```
Which can then be invoked as:

```posh
▶ .\build.ps1 --bootstrap; .\build.ps1 
Preparing to run build script...
Running build script...
Preparing to run build script...
Running build script...

========================================
Build
========================================
0.3.0-alpha1-0002-c6c70600

========================================
Default
========================================

Task                          Duration
--------------------------------------------------
Build                         00:00:00.3256116
--------------------------------------------------
Total:                        00:00:00.3304598
```
> You change the arguments passed to `SimpleVersion` using the [ProcessSettings] class.

[Results]: ./results.md
[dotnet cli]: https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools
[Cake.DotNetTool.Module]: https://www.gep13.co.uk/blog/introducing-cake.dotnettool.module
[ProcessSettings]: https://cakebuild.net/api/Cake.Core.IO/ProcessSettings/