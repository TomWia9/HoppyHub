terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.97.0"
    }
    random = {
      source  = "hashicorp/random"
      version = ">= 3.6.0"
    }
    github = {
      source  = "integrations/github"
      version = "~> 6.0"
    }
  }
}
provider "azurerm" {
  features {}
}
provider "github" {}