## List of contents

- ### [Description](#description)

- ### [Stack](#stack)

- ### [Installation & Configuration](#installation&configuration)

- ### [Configuration](#configuration)

- ### [Endpoints](#endpoints)

- ### [Tests](#tests)

- ### [License](#license)

## Description

**HoppyHub** is a **.NET 8** and **Angular** implemented Web Application for exchanging opinions about different types of beer. Users can create accounts, add beers to their favorites list, rate and comment on beers.

## Stack

### Development Stack:

- **Entity Framework Core** - database ORM

- **MediatR** - for mediator and CQRS patterns

- **MassTransit** - for message brokers

- **JwtBearer** - for authorization

- **AutoMapper** - for mapping DTO-s and EntityModels data

- **Fluent Validation** - for data validation

- **xUnit** - for tests

### Azure Stack:

- **Azure Sql Server and Sql Databases** - for storing data

- **Azure Service Bus** - for message broker

- **Azure Storage Account** - for storing images

- **Azure KeyVault** - for storing sensitive data and access keys

- **Azure App services** - for hosting apis

- **Azure Static Web App** - for hosting UI app

### Other:

- **Docker** - for application containerization

- **RabbitMQ** - for a local message broker, since azure service bus cannot be use localy

- **Terraform** - for creating Azure infrastructure

## Installation & Configuration

### Local

Make sure you have the **.NET 8.0 SDK**, **Microsoft SQL Server**, **RabbitMQ**, **NodeJs** installed on your machine.\
Go to **Services** directory.

Set these appsettings values in all solutions:

- **JwtSettings:Secret** - Secret value for json web token

- **TempBeerImageUri** - Temporary beer image uri

- **ContainerName** - Azure storage account container name

- **ConnectionStrings:StorageAccountConnection** - Azure Storage Accoutnt connection string

- **ConnectionStrings:\*DbConnection** - Database connection string

- **UIAppUrl** - UI app URL (default: localhost:4200)

- **RabbitMQ:Host** - RabbitMQ host (default: localhost)

- **RabbitMQ:Username** - RabbitMQ username (default: guest)

- **RabbitMQ:Password** - RabbitMQ password (default: guest)

Initialize databases data using **Scripts\RestoreDatabases.ps1** script.\
You can specify **@ImageUri** varaibles in **Scripts\SQLScripts\OpinionManagement_Opinions.sql** and **Scripts\SQLScripts\BeerManagement_Beers.sql** first.\

Build and run all solutions (`dotnet run` in solution root directory)\
Build and run UI App (`ng serve -o` in Apps\hoppy-hub directory)

### Docker

Make sure you have **Docker** installed on your machine.\
Open docker-compose.yml file.
Set proper **TempBeerImageUri**, **ConnectionStrings\_\_StorageAccountConnection** and **ContainerName** environment variables.\
Execute `docker compose up` command in root directory.\
Initialize databases data using **Scripts\RestoreDatabases.ps1** script with -docker flag.\
You can specify **@ImageUri** varaibles in **Scripts\SQLScripts\OpinionManagement_Opinions.sql** and **Scripts\SQLScripts\BeerManagement_Beers.sql** first.\

### Azure

Make sure you have **Terraform** installed on your machinge.\
Go to **Azure Infrastructure** directory.\
Adjust variables for your purpose.\
Execute `terraform init` and `terraform apply` commands.
Adjust **.github\workflows\deploy_hoppyhub.yml** pipeline and run it.

## Endpoints

JWT token must be provided in the header when accessing sensitive data.

//TODO

## Tests

This project uses unit tests provided by xUnit.

To run tests go to the root solution directory and do:

`dotnet test`

## License

[MIT](https://choosealicense.com/licenses/mit/)
