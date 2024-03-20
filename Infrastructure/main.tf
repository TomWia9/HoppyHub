locals {
  resource_group_name  = "HoppyHub1"
  key_vault_name       = "HoppyHub-KeyVault1"
  storage_account_name = "hoppyhub1"
  blob_container_name = "hoppyhub-container"
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
  purge_protection_enabled    = false

  sku_name = "standard"

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    secret_permissions = [
      "Get", "List", "Set", "Delete", "Purge"
    ]
  }
}

#Storage account, container, blob
resource "azurerm_storage_account" "storage_account" {
  name                     = local.storage_account_name
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}
resource "azurerm_storage_container" "blob_container" {
  name                  = local.blob_container_name
  storage_account_name  = azurerm_storage_account.storage_account.name
  container_access_type = "blob"
}
resource "azurerm_storage_blob" "temp_beer_image" {
  name                   = "Beers/temp.jpg"
  storage_account_name   = azurerm_storage_account.storage_account.name
  storage_container_name = azurerm_storage_container.blob_container.name
  type                   = "Block"
  source                 = "./assets/temp.jpg"
}

#Key Vault secrets
resource "azurerm_key_vault_secret" "kv_secret_temp_beer_image_url" {
  name         = "TempBeerImageUri"
  value        = azurerm_storage_blob.temp_beer_image.url
  key_vault_id = azurerm_key_vault.key_vault.id
}
resource "azurerm_key_vault_secret" "kv_secret_jwt" {
  name         = "JwtSettings--Secret"
  value        = "fsaf2faf2j83f2389fj2qa9sj392jwfqa92fjqwi21rfqwa"
  key_vault_id = azurerm_key_vault.key_vault.id
}
resource "azurerm_key_vault_secret" "kv_secret_blob_connection_string" {
  name         = "BlobContainerSettings--BlobConnectionString"
  value        = azurerm_storage_account.storage_account.primary_connection_string
  key_vault_id = azurerm_key_vault.key_vault.id
}