# Sharpie
*A C# package manager without the useless fluff.*

**This repository is now being maintained as part of the Algo project, [here.](http://github.com/c272/algo-lang) This repository is no longer updated.**

## Getting Started
To get started adding packages to your C# projects or PATH, first download the latest release of Sharpie from the [releases tab](https://github.com/c272/sharpie/releases) on GitHub. When this is done, extract the release into a folder, and then add that folder to the PATH environment variable.

(*If you don't know how to do that, you can check [here.](https://docs.alfresco.com/4.2/tasks/fot-addpath.html)*)

When this is done, execute the command `sharpie` in the console, and then add the `sharpie\packages\` directory to PATH. After this, you can simply use `sharpie help` to get more information on commands. Below is a short description of how to install packages.

## Sources and Packages
#### Sources
Before adding packages, you must first add sources which you can draw packages from. Think of these as massive collections of packages which Sharpie pulls from when looking to install a package. If it finds a matching package in a source, it will attempt to download it. However, without any sources added, there are no packages to draw from.

To add a source to Sharpie, you can use the command `sharpie sources add [link]`.

For more information on sources, you can use the command reference held at `sharpie help sources`.

#### Packages
Once one or more sources have been added, you can install and modify packages from those sources. To add a package such as a class library or `.cs` package to a C# project, you must **navigate to the project directory in console,** and then use `sharpie add [pkgname]`. If you want to add a CLI package, this can be done from anywhere, and uses the same command.

For more information on packages, you can use the command reference held at `sharpie help packages`.

## Creating a Source
To create a source for Sharpie, there is a very simple schema to follow. Below is an outline of a basic source, which you can edit to fit your needs. Any spaces in the source name will be omitted when installing, modifying and removing the source.

```
SourceName
cliPackageName | http://package.link/directlyto.exe | exe
classLibraryPackageName | http://package.link/directlyto.zip | zip
```

Currently, packages can come in two forms, `exe`, which are always extracted directly to PATH and assumed as CLI packages, and `zip`, which are assumed as C# project packages, and extracted into the `projectdir\packages\[pkgname]` directory.
