locals {
  resource_group_name = "HoppyHub"
  key_vault_name      = "HoppyHub-KeyVault"
}

#Resource Group
resource "azurerm_resource_group" "rg" {
  name     = local.resource_group_name
  location = var.location
  tags = {
    Environment = var.environment
  }
}

#Key Vault
data "azurerm_client_config" "current" {}

resource "azurerm_key_vault" "key_vault" {
  name                        = local.key_vault_name
  location                    = azurerm_resource_group.rg.location
  resource_group_name         = azurerm_resource_group.rg.name
  enabled_for_disk_encryption = true
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  soft_delete_retention_days  = 7
  purge_protection_enabled    = false

  sku_name = "standard"

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    secret_permissions = [
      "Get", "List", "Set"
    ]
  }
}

module "azure_keyvault_secrets" {
  source = "./modules/azure-keyvault-secrets"

  key_vault_id = azurerm_key_vault.key_vault.id
}