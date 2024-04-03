variable "db_name" {
  type        = string
  description = "Database name"
}

variable "sql_server_id" {
  type        = string
  description = "SQL Server id"
}

variable "sql_server_full_name" {
  type        = string
  description = "SQL Server full name"
}

variable "sql_server_user" {
  type        = string
  description = "SQL Server user"
}

variable "sql_server_password" {
  type        = string
  description = "SQL Server password"
}

variable "connection_string_secret_name" {
  type        = string
  description = "Connection string secret name"
}

variable "key_vault_id" {
  type        = string
  description = "Key vault id"
}

variable "storage_key" {
  type        = string
  description = "Storage key"
}

variable "bacpac_uri" {
  type        = string
  description = "Bacpac uri"
}