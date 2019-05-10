Introduction
============

SimpleVersion is available as a command line tool (distributed via nuget), a Cake
addin, or as a collection of libraries for direct consumption in a project.

To get a better understanding of the values returned from invoking SimpleVersion
see the [results documentation][results].

Command Line Tool
-----------------

The latest version of the command line tool can be found on nuget.org.
You can install SimpleVersion.Command using `nuget.exe`:

```posh
# Installs the latest version to the current directory
nuget install SimpleVersion.Command
```

> [!INFO]
> Execute `nuget install -?` for additional options

Once downloaded, you can invoke SimpleVersion using the `exe` under the installed
`tools` folder.

```posh
.\SimpleVersion.Command<version>\tools\SimpleVersion.exe
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
