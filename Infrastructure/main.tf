locals {
    resource_group_name = "HoppyHub"
}

#Resource Groups
resource "azurerm_resource_group" "rg" {
  name     = local.resource_group_name
  location = var.location
  tags = {
    Environment = var.environment
  }
}