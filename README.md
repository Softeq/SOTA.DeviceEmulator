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

## Deployment

The application is built as a ClickOnce package and published to an Azure Blob storage.

Make sure the necessary resources are created in an Azure subscription:

* Resource group named `sota`
* Blob storage account named `sotaops`
* Blob storage container named `terraform-state`

You can use the following command samples to create all listed things:

```powershell
# Login and select subscription
az login
az account set --subscription <your-subscription-id>

# Create resource group
az group create --name sota --location eastus

# Create blob storage account
az storage account create --name sotaops --resource-group sota --location eastus --sku Standard_LRS

# Create blob storage container
az storage container create --name terraform-state --account-name sotaops
```

Then, use deploy script to upload artifacts to the blob storage

```powershell
./build Deploy
```
