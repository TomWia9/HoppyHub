resource "azurerm_linux_web_app" "web_app" {
  name                = var.name
  resource_group_name = var.resource_group_name
  location            = var.location
  service_plan_id     = var.service_plan_id

  site_config {
    application_stack {
      dotnet_version = "8.0"
    }
    cors {
      allowed_origins = [var.static_web_app_uri]
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
  key_vault_id       = var.key_vault_id
  tenant_id          = data.azurerm_client_config.current.tenant_id
  object_id          = azurerm_linux_web_app.web_app.identity.0.principal_id
  secret_permissions = ["Get", "List"]
}

resource "azurerm_key_vault_secret" "kv_secret_app_url" {
  name         = var.app_url_secret_name
  value        = "${azurerm_linux_web_app.web_app.name}.azurewebsites.net"
  key_vault_id = var.key_vault_id
}

resource "github_actions_secret" "gh_secret_publish_profile" {
  repository      = "HoppyHub"
  secret_name     = var.app_publish_profile_secret_name
  plaintext_value = "<publishData><publishProfile profileName=\"${azurerm_linux_web_app.web_app.name} - Zip Deploy\" publishMethod=\"ZipDeploy\" publishUrl=\"${azurerm_linux_web_app.web_app.name}.scm.azurewebsites.net:443\" userName=\"${azurerm_linux_web_app.web_app.site_credential[0].name}\" userPWD=\"${azurerm_linux_web_app.web_app.site_credential[0].password}\" destinationAppUrl=\"http://${azurerm_linux_web_app.web_app.name}.azurewebsites.net\" controlPanelLink=\"https://portal.azure.com\" webSystem=\"WebSites\"></publishProfile></publishData>"
}