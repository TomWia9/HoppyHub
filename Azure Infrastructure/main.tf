locals {
  resource_group_name      = "${var.master_name}rg"
  key_vault_name           = "${var.master_name}kv"
  storage_account_name     = "${var.master_name}sa"
  blob_container_name      = "${var.master_name}container"
  service_bus_name         = "${var.master_name}servicebus"
  sql_server_name          = "${var.master_name}sqlserver"
  app_service_plan_name    = "${var.master_name}asp"
  web_app_name             = "${var.master_name}staticwebapp"
  user_management_name     = "UserManagement"
  beer_management_name     = "BeerManagement"
  opinion_management_name  = "OpinionManagement"
  favorite_management_name = "FavoriteManagement"
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

#Storage account, container, blobs
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
resource "azurerm_storage_blob" "user_management_db_bacpac" {
  name                   = "Bacpacs/UserManagement.bacpac"
  storage_account_name   = azurerm_storage_account.storage_account.name
  storage_container_name = azurerm_storage_container.blob_container.name
  type                   = "Block"
  source                 = "./assets/Bacpacs/UserManagement.bacpac"
}
resource "azurerm_storage_blob" "beer_management_db_bacpac" {
  name                   = "Bacpacs/BeerManagement.bacpac"
  storage_account_name   = azurerm_storage_account.storage_account.name
  storage_container_name = azurerm_storage_container.blob_container.name
  type                   = "Block"
  source                 = "./assets/Bacpacs/BeerManagement.bacpac"
}
resource "azurerm_storage_blob" "opinion_management_db_bacpac" {
  name                   = "Bacpacs/OpinionManagement.bacpac"
  storage_account_name   = azurerm_storage_account.storage_account.name
  storage_container_name = azurerm_storage_container.blob_container.name
  type                   = "Block"
  source                 = "./assets/Bacpacs/OpinionManagement.bacpac"
}
resource "azurerm_storage_blob" "favorite_management_db_bacpac" {
  name                   = "Bacpacs/FavoriteManagement.bacpac"
  storage_account_name   = azurerm_storage_account.storage_account.name
  storage_container_name = azurerm_storage_container.blob_container.name
  type                   = "Block"
  source                 = "./assets/Bacpacs/FavoriteManagement.bacpac"
}

#Service bus
resource "azurerm_servicebus_namespace" "service_bus" {
  name                = local.service_bus_name
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "Standard"
}

#Sql Server, databases
resource "random_password" "sql_server_password" {
  length           = 16
  special          = true
  min_lower        = 1
  min_upper        = 1
  min_numeric      = 1
  min_special      = 1
  override_special = "!$#%"
}
resource "azurerm_mssql_server" "sql_server" {
  name                         = local.sql_server_name
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = "sqladmin"
  administrator_login_password = random_password.sql_server_password.result
}
module "user_management_db" {
  source                        = "./modules/sql_database"
  db_name                       = local.user_management_name
  sql_server_id                 = azurerm_mssql_server.sql_server.id
  sql_server_full_name          = azurerm_mssql_server.sql_server.fully_qualified_domain_name
  sql_server_user               = azurerm_mssql_server.sql_server.administrator_login
  sql_server_password           = azurerm_mssql_server.sql_server.administrator_login_password
  connection_string_secret_name = "ConnectionStrings--${local.user_management_name}DbConnection"
  key_vault_id                  = azurerm_key_vault.key_vault.id
}
module "beer_management_db" {
  source                        = "./modules/sql_database"
  db_name                       = local.beer_management_name
  sql_server_id                 = azurerm_mssql_server.sql_server.id
  sql_server_full_name          = azurerm_mssql_server.sql_server.fully_qualified_domain_name
  sql_server_user               = azurerm_mssql_server.sql_server.administrator_login
  sql_server_password           = azurerm_mssql_server.sql_server.administrator_login_password
  connection_string_secret_name = "ConnectionStrings--${local.beer_management_name}DbConnection"
  key_vault_id                  = azurerm_key_vault.key_vault.id
}
module "opinion_management_db" {
  source                        = "./modules/sql_database"
  db_name                       = local.opinion_management_name
  sql_server_id                 = azurerm_mssql_server.sql_server.id
  sql_server_full_name          = azurerm_mssql_server.sql_server.fully_qualified_domain_name
  sql_server_user               = azurerm_mssql_server.sql_server.administrator_login
  sql_server_password           = azurerm_mssql_server.sql_server.administrator_login_password
  connection_string_secret_name = "ConnectionStrings--${local.opinion_management_name}DbConnection"
  key_vault_id                  = azurerm_key_vault.key_vault.id
}
module "favorite_management_db" {
  source                        = "./modules/sql_database"
  db_name                       = local.favorite_management_name
  sql_server_id                 = azurerm_mssql_server.sql_server.id
  sql_server_full_name          = azurerm_mssql_server.sql_server.fully_qualified_domain_name
  sql_server_user               = azurerm_mssql_server.sql_server.administrator_login
  sql_server_password           = azurerm_mssql_server.sql_server.administrator_login_password
  connection_string_secret_name = "ConnectionStrings--${local.favorite_management_name}DbConnection"
  key_vault_id                  = azurerm_key_vault.key_vault.id
}

#App service plan add app services
resource "azurerm_service_plan" "app_service_plan" {
  name                = local.app_service_plan_name
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  os_type             = "Linux"
  sku_name            = "B1"
}
module "user_management_app" {
  source              = "./modules/web_app"
  web_app_name        = "hoppy-hub-${local.user_management_name}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  service_plan_id     = azurerm_service_plan.app_service_plan.id
  app_url_secret_name = "${local.user_management_name}ApiUrl"
  key_vault_id        = azurerm_key_vault.key_vault.id
  key_vault_name      = azurerm_key_vault.key_vault.name
}
module "beer_management_app" {
  source              = "./modules/web_app"
  web_app_name        = "hoppy-hub-${local.beer_management_name}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  service_plan_id     = azurerm_service_plan.app_service_plan.id
  app_url_secret_name = "${local.beer_management_name}ApiUrl"
  key_vault_id        = azurerm_key_vault.key_vault.id
  key_vault_name      = azurerm_key_vault.key_vault.name
}
module "opinion_management_app" {
  source              = "./modules/web_app"
  web_app_name        = "hoppy-hub-${local.opinion_management_name}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  service_plan_id     = azurerm_service_plan.app_service_plan.id
  app_url_secret_name = "${local.opinion_management_name}ApiUrl"
  key_vault_id        = azurerm_key_vault.key_vault.id
  key_vault_name      = azurerm_key_vault.key_vault.name
}
module "favorite_management_app" {
  source              = "./modules/web_app"
  web_app_name        = "hoppy-hub-${local.favorite_management_name}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  service_plan_id     = azurerm_service_plan.app_service_plan.id
  app_url_secret_name = "${local.favorite_management_name}ApiUrl"
  key_vault_id        = azurerm_key_vault.key_vault.id
  key_vault_name      = azurerm_key_vault.key_vault.name
}

resource "azurerm_static_web_app" "ui_static_web_app" {
  name                = local.web_app_name
  resource_group_name = azurerm_resource_group.rg.name
  location            = "westeurope"
}

#Key Vault secrets
resource "random_password" "random_jwt" {
  length  = 32
  special = false
}
resource "azurerm_key_vault_secret" "kv_secret_temp_beer_image_url" {
  name         = "TempBeerImageUri"
  value        = azurerm_storage_blob.temp_beer_image.url
  key_vault_id = azurerm_key_vault.key_vault.id
}
resource "azurerm_key_vault_secret" "kv_secret_jwt" {
  name         = "JwtSettings--Secret"
  value        = random_password.random_jwt.result
  key_vault_id = azurerm_key_vault.key_vault.id
}
resource "azurerm_key_vault_secret" "kv_secret_storage_account_connection_string" {
  name         = "ConnectionStrings--StorageAccountConnection"
  value        = azurerm_storage_account.storage_account.primary_connection_string
  key_vault_id = azurerm_key_vault.key_vault.id
}
resource "azurerm_key_vault_secret" "kv_secret_container_name" {
  name         = "ContainerName"
  value        = azurerm_storage_container.blob_container.name
  key_vault_id = azurerm_key_vault.key_vault.id
}
resource "azurerm_key_vault_secret" "kv_secret_service_bus_connection_string" {
  name         = "ConnectionStrings--AzureServiceBusConnection"
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
  value        = azurerm_mssql_server.sql_server.administrator_login_password
  key_vault_id = azurerm_key_vault.key_vault.id
}