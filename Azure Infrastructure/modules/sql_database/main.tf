resource "azurerm_mssql_database" "db" {
  name                 = var.db_name
  server_id            = var.sql_server_id
  collation            = "SQL_Latin1_General_CP1_CI_AS"
  license_type         = "LicenseIncluded"
  max_size_gb          = 2
  sku_name             = "Basic"
  enclave_type         = "VBS"
  storage_account_type = "Local"
  import {
    storage_uri                  = var.bacpac_uri
    storage_key                  = var.storage_key
    storage_key_type             = "StorageAccessKey"
    administrator_login          = var.sql_server_user
    administrator_login_password = var.sql_server_password
    authentication_type          = "Sql"
  }
}

resource "azurerm_key_vault_secret" "kv_secret_db_connection_string" {
  name         = var.connection_string_secret_name
  value        = "Server=${var.sql_server_full_name};Database=${var.db_name};User=${var.sql_server_user};Password=${var.sql_server_password};MultipleActiveResultSets=True;"
  key_vault_id = var.key_vault_id
}