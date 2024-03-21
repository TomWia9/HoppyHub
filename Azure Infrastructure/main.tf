locals {
  resource_group_name         = "hoppy-hub"
  key_vault_name              = "hoppy-hub-kv"
  storage_account_name        = "hoppyhubsa6"
  blob_container_name         = "hoppy-hub-container"
  service_bus_name            = "hoppy-hub-service-bus"
  sql_server_name             = "hoppy-hub-sql-server"
  user_management_db_name     = "UserManagement"
  beer_management_db_name     = "BeerManagement"
  opinion_management_db_name  = "OpinionManagement"
  favorite_management_db_name = "FavoriteManagement"
}

#Resource Group
resource "azurerm_resource_group" "rg" {
  name     = local.resource_group_name
  location = var.location
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

#Service bus
resource "azurerm_servicebus_namespace" "service_bus" {
  name                = local.service_bus_name
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "Standard"
}

#Sql Server, databases
resource "random_password" "password" {
  length           = 16
  special          = true
  override_special = "_@%"
}
resource "azurerm_mssql_server" "sql_server" {
  name                         = local.sql_server_name
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = "sqladmin"
  administrator_login_password = random_password.password.result
}
module "user_management_db" {
  source                        = "./modules/sql_database"
  db_name                       = local.user_management_db_name
  sql_server_id                 = azurerm_mssql_server.sql_server.id
  sql_server_full_name          = azurerm_mssql_server.sql_server.fully_qualified_domain_name
  sql_server_user               = azurerm_mssql_server.sql_server.administrator_login
  sql_server_password           = azurerm_mssql_server.sql_server.administrator_login_password
  connection_string_secret_name = "UserManagementDbConnectionString"
  key_vault_id                  = azurerm_key_vault.key_vault.id
}
module "beer_management_db" {
  source                        = "./modules/sql_database"
  db_name                       = local.beer_management_db_name
  sql_server_id                 = azurerm_mssql_server.sql_server.id
  sql_server_full_name          = azurerm_mssql_server.sql_server.fully_qualified_domain_name
  sql_server_user               = azurerm_mssql_server.sql_server.administrator_login
  sql_server_password           = azurerm_mssql_server.sql_server.administrator_login_password
  connection_string_secret_name = "BeerManagementDbConnectionString"
  key_vault_id                  = azurerm_key_vault.key_vault.id
}
module "opinion_management_db" {
  source                        = "./modules/sql_database"
  db_name                       = local.opinion_management_db_name
  sql_server_id                 = azurerm_mssql_server.sql_server.id
  sql_server_full_name          = azurerm_mssql_server.sql_server.fully_qualified_domain_name
  sql_server_user               = azurerm_mssql_server.sql_server.administrator_login
  sql_server_password           = azurerm_mssql_server.sql_server.administrator_login_password
  connection_string_secret_name = "OpinionManagementDbConnectionString"
  key_vault_id                  = azurerm_key_vault.key_vault.id
}
module "favorite_management_db" {
  source                        = "./modules/sql_database"
  db_name                       = local.favorite_management_db_name
  sql_server_id                 = azurerm_mssql_server.sql_server.id
  sql_server_full_name          = azurerm_mssql_server.sql_server.fully_qualified_domain_name
  sql_server_user               = azurerm_mssql_server.sql_server.administrator_login
  sql_server_password           = azurerm_mssql_server.sql_server.administrator_login_password
  connection_string_secret_name = "FavoriteManagementDbConnectionString"
  key_vault_id                  = azurerm_key_vault.key_vault.id
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
resource "azurerm_key_vault_secret" "kv_secret_service_bus_connection_string" {
  name         = "AzureServiceBus--ConnectionString"
  value        = azurerm_servicebus_namespace.service_bus.default_primary_connection_string
  key_vault_id = azurerm_key_vault.key_vault.id
}
resource "azurerm_key_vault_secret" "kv_secret_sql_server_name" {
  name         = "SqlServerName"
  value        = azurerm_mssql_server.sql_server.fully_qualified_domain_name
  key_vault_id = azurerm_key_vault.key_vault.id
}
resource "azurerm_key_vault_secret" "kv_secret_sql_server_user" {
  name         = "SqlServerUser"
  value        = "sqladmin"
  key_vault_id = azurerm_key_vault.key_vault.id
}
resource "azurerm_key_vault_secret" "kv_secret_sql_server_password" {
  name         = "SqlServerPassword"
  value        = random_password.password.result
  key_vault_id = azurerm_key_vault.key_vault.id
}