# SOTA Device Emulator

[![Build Status](https://dev.azure.com/SofteqDevelopment/SOTA/_apis/build/status/Device%20Emulator?branchName=develop)](https://dev.azure.com/SofteqDevelopment/SOTA/_build/latest?definitionId=49&branchName=develop)

A desktop application that emulates a device in an IoT scenario. The repository belongs to the [SOTA](https://portal.softeq.com/display/SOTA) project.

## Development

Make sure that the following dependencies are installed:

* [Visual Studio 2019](https://visualstudio.microsoft.com/downloads/)
* [.NET Core 2.2 SDK](https://dotnet.microsoft.com/download/dotnet-core)
* [Azure CLI](https://docs.microsoft.com/en-us/cli/azure)
* [Terraform](https://chocolatey.org/packages/terraform)

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

To make a release, the following conditions should be satisfied:

* The branch should be either **develop**, **master**, **release** (e.g `release/1.1.0`) or **hotfix** (e.g. `hotfix/1.1.1`).
* For **release** and **hotfix** branches the commit message should contain **`[ci release]`** in the commit message. You can add that to a merge comment when merging a PR or by pushing an empty commit containing the mentioned message.

From the developer perspective the release flow can be described as follows:

1. Develop and merge several feature or bugfix pull requests targeting develop branch (`feature/XXXX_[feature-name] → develop`).
2. On code freeze day create a release branch and open a release PR (`release/x.x.x → master`).
3. Fix known critical and major bugs using bugfix PRs targeting release branch (`bugfix/XXXX_[bug-name] → release/x.x.x`). 
4. Make a beta release when merging last bugfix PR by including **`[ci release]`** in the merge commit message.
5. Wait until regression and user acceptance testing is finished.
6. Make a stable release by merging release PR into `master`.
