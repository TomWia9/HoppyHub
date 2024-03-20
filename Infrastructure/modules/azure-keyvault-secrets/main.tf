resource "azurerm_key_vault_secret" "kv_secret_jwt" {
  name         = "JwtSettings--Secret"
  value        = "fsaf2faf2j83f2389fj2qa9sj392jwfqa92fjqwi21rfqwa"
  key_vault_id = var.key_vault_id
}

resource "azurerm_key_vault_secret" "kv_secret_temp_beer_image_uri" {
  name         = "TempBeerImageUri"
  value        = ""
  key_vault_id = var.key_vault_id
}

#Add more secrets here