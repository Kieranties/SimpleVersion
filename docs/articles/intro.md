Introduction
============

SimpleVersion is available as a cross-platform dotnet tool, a netstandard Cake
addin, or as a collection of libraries for direct consumption in a project.

To get a better understanding of the values returned from invoking SimpleVersion
see the [results documentation][results].

Dotnet Tool
-----------------

The latest version of the dotnet tool can be found on nuget.org.
You can install SimpleVersion.Tool the [dotnet cli]:

```posh
# Installs the latest version as a global tool
dotnet install -g SimpleVersion.Tool
```

Once installed, you can invoke SimpleVersion in your favourite shell, optionally passing the path to your repository (defaults to current working directory)

```posh
PS c:\MyRepo> simpleversion
PS c:\> simpleversion ./MyRepo
```

Cake Addin
----------

The `Cake.SimpleVersion` addin can be used in a Cake build script to invoke
SimpleVersion.

A simple `build.cake` can invoke the `SimpleVersion`:

```c#
// Include the Cake addin
#addin nuget:?package=Cake.SimpleVersion&prerelease
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var versionFormat = Argument("versionFormat", "Semver1");

// Call SimpleVersion to get version information
var versionInfo = SimpleVersion();

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Build")
    .Does(() =>
{
      // Display version - Access formats dictionary or other properties
      Information($"My {versionFormat} is: {versionInfo.Formats[versionFormat]}");
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


[Results]: ./results.md
[dotnet cli]: https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools