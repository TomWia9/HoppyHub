resource "azurerm_linux_web_app" "web_app" {
  name                = var.web_app_name
  resource_group_name = var.resource_group_name
  location            = var.location
  service_plan_id     = var.service_plan_id

  site_config {
    application_stack {
      dotnet_version = "8.0"
    }
  }

  app_settings = {
    "KeyVaultName" = var.key_vault_name
  }
  identity {
    type = "SystemAssigned"
  }
}

data "azurerm_client_config" "current" {}

resource "azurerm_key_vault_access_policy" "kv_access" {
  key_vault_id        = var.key_vault_id
  tenant_id           = data.azurerm_client_config.current.tenant_id
  object_id           = azurerm_linux_web_app.web_app.identity.0.principal_id
  secret_permissions  = ["Get", "List"]
}

resource "azurerm_key_vault_secret" "kv_secret_app_url" {
  name         = var.app_url_secret_name
  value        = "${azurerm_linux_web_app.web_app.name}.azurewebsites.net"
  key_vault_id = var.key_vault_id
}