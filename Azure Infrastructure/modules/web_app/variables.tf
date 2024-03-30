variable "name" {
  type        = string
  description = "Name"
}

variable "static_web_app_uri" {
  type        = string
  description = "Static web app uri"
}

variable "resource_group_name" {
  type        = string
  description = "Resource group name"
}

variable "location" {
  type        = string
  description = "Location"
}

variable "service_plan_id" {
  type        = string
  description = "Service plan id"
}

variable "app_url_secret_name" {
  type        = string
  description = "App url secret name"
}

variable "key_vault_id" {
  type        = string
  description = "Key vault id"
}

variable "key_vault_name" {
  type        = string
  description = "Key vault name"
}