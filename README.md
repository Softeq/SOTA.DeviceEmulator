# SOTA Device Emulator

[![Build Status](https://dev.azure.com/SofteqDevelopment/SOTA/_apis/build/status/Device%20Emulator?branchName=develop)](https://dev.azure.com/SofteqDevelopment/SOTA/_build/latest?definitionId=49&branchName=develop)

A desktop application that emulates a device in an IoT scenario. The repository belongs to the [SOTA](https://portal.softeq.com/display/SOTA) project.

## Installation

- Make sure the [certificates from the Softeq portal](https://portal.softeq.com/display/SOTA/Environments+and+Credentials) are registered in the OS certificates user-personal store (`Cert:/CurrentUser/My` in PowerShell).
- Download the needed build from:
  - [Azure DevOps Builds](https://dev.azure.com/SofteqDevelopment/SOTA/_build?definitionId=49) (all builds available). Downloads are performed using the [Azure CLI extension](https://github.com/Azure/azure-devops-cli-extension).
  - [GitHub Releases](https://github.com/Softeq/SOTA.DeviceEmulator/releases) (beta and stable releases).
- Unpack the binaries at any place of choice in your storage device.
- Run the "SOTA.DeviceEmulator.exe" which you can find in the unpacked root directory.
- You should be able to generate a new device certificate and connect to the IoT hub now from within the application.

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

## Releases

The application binaries are published to an internal [Azure Artifacts feed](https://dev.azure.com/SofteqDevelopment/SOTA/_packaging?_a=feed&feed=SOTA). Beta and stable releases are also published to GitHub.

From the developer perspective the release flow can be described as follows:

1. Develop and merge several feature or bugfix pull requests targeting develop branch (`feature/XXXX_[feature-name] → develop`).
2. On code freeze day create a release branch and open a release PR (`release/x.x.x → master`).
3. Fix known critical and major bugs using bugfix PRs targeting release branch (`bugfix/XXXX_[bug-name] → release/x.x.x`). 
4. Create and publish beta release in Azure Pipelines.
5. Wait until regression and user acceptance testing is finished.
6. Merge release branch to master and develop.
6. Approve to publish stable release in Azure Pipelines.
