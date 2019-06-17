# SOTA Device Emulator

A desktop application that emulates a device in an IoT scenario. The repository belongs to the [SOTA](https://portal.softeq.com/display/SOTA) project.

## Development

Make sure that the following dependencies are installed:

* [Visual Studio 2019](https://visualstudio.microsoft.com/downloads/)
* [.NET Core 2.2 SDK](https://dotnet.microsoft.com/download/dotnet-core)

Also, there are several extensions and applications that you may find useful during development:

* [JetBrains ReSharper](https://www.jetbrains.com/resharper) to keep code clean and perform complex refactoring. In case you are using ReSharper, make sure solution settings from the repository are used.
* [Nuke IDE Extensions](https://nuke.build/docs/running-builds/from-ides.html) to run and debug Nuke targets from your favorite IDE.
* [Markdown Editor](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.MarkdownEditor) is a Visual Studio extension to write beautiful documentation with Markdown.

After installing all required dependencies, make sure you can build the project and run tests.

```powershell
./build
# Or you can do the same with
./build LocalBuild
```
