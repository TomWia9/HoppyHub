terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.11.0"
    }
    random = {
      source  = "hashicorp/random"
      version = ">= 3.6.0"
    }
    github = {
      source  = "integrations/github"
      version = "~> 6.4.0"
    }
  }
}
provider "azurerm" {
  features {}
}
provider "github" {}