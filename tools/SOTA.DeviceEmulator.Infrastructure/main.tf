locals {
  project_name = "sota"
  component_name = "deviceemulator"
}

variable "region" {
  default = "East US"
}

provider "azurerm" {
  version = "=1.28.0"
}

terraform {
  backend "azurerm" {
    storage_account_name = "sotaops"
    resource_group_name = "sota"
    container_name       = "terraform-state"
    key                  = "deviceemulator.tfstate"
  }
}

resource "azurerm_storage_account" "artifacts_storage" {
  name                     = "${local.project_name}${local.component_name}"
  resource_group_name      = "${local.project_name}"
  location                 = "${var.region}"
  account_tier             = "Standard"
  account_kind             = "StorageV2"
  account_replication_type = "LRS"

  provisioner "local-exec" {
    command = "az extension add --name storage-preview | az storage blob service-properties update --account-name ${azurerm_storage_account.artifacts_storage.name} --static-website --index-document index.html --404-document index.html"
  }
}

output "artifacts_storage_account_name" {
  value = "${azurerm_storage_account.artifacts_storage.name}"
}
output "artifacts_storage_connection_string" {
  value = "${azurerm_storage_account.artifacts_storage.primary_blob_connection_string}"
}