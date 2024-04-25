# Project Title

A brief description of what this project does and who it's for

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

### UserManagement Service

#### Identity Controller

| Method | Path                   | Body                                                                                                                   | Params | Description                              | Responses                                                                                                    | Who can access |
| :----- | :--------------------- | :--------------------------------------------------------------------------------------------------------------------- | :----- | :--------------------------------------- | :----------------------------------------------------------------------------------------------------------- | :------------- |
| POST   | /api/Identity/register | [RegisterUserCommand](./Services/UserManagement/src/Application/Identity/Commands/RegisterUser/RegisterUserCommand.cs) | None   | Creates a new user and returns JWT token | [AuthenticationResult](./Services/UserManagement/src/Application/Common/Models/AuthenticationResult.cs), 400 | Everyone       |
| POST   | /api/Identity/login    | [LoginUserCommand](./Services/UserManagement/src/Application/Identity/Commands/LoginUser/LoginUserCommand.cs)          | None   | Returns JWT token                        | [AuthenticationResult](./Services/UserManagement/src/Application/Common/Models/AuthenticationResult.cs), 400 | Everyone       |

#### Users Controller

| Method | Path            | Body                                                                                                          | Params                                                                                             | Description      | Responses                                                                                 | Who can access               |
| :----- | :-------------- | :------------------------------------------------------------------------------------------------------------ | :------------------------------------------------------------------------------------------------- | :--------------- | :---------------------------------------------------------------------------------------- | :--------------------------- |
| GET    | /api/Users      | None                                                                                                          | [GetUsersQuery](./Services/UserManagement/src/Application/Users/Queries/GetUsers/GetUsersQuery.cs) | Gets all users   | PaginatedList<[UserDto](./Services/UserManagement/src/Application/Users/Dtos/UserDto.cs)> | Everyone                     |
| GET    | /api/Users/{id} | None                                                                                                          | Id:GUID                                                                                            | Gets user by id  | [UserDto](./Services/UserManagement/src/Application/Users/Dtos/UserDto.cs), 404           | Everyone                     |
| PUT    | /api/Users/{id} | [UpdateUserCommand](./Services/UserManagement/src/Application/Users/Commands/UpdateUser/UpdateUserCommand.cs) | Id:GUID                                                                                            | Updates the user | 204, 400, 404, 401, 403                                                                   | Account owner, Administrator |
| DELETE | /api/Users      | [DeleteUserCommand](./Services/UserManagement/src/Application/Users/Commands/DeleteUser/DeleteUserCommand.cs) | Id:GUID                                                                                            | Deletes the user | 204, 400, 404, 401, 403                                                                   | Account owner, Administrator |

### BeerManagement Service

#### Beers Controller

| Method | Path            | Body                                                                                                                              | Params                                                                                             | Description                                        | Responses                                                                                 | Who can access |
| :----- | :-------------- | :-------------------------------------------------------------------------------------------------------------------------------- | :------------------------------------------------------------------------------------------------- | :------------------------------------------------- | :---------------------------------------------------------------------------------------- | :------------- |
| GET    | /api/Beers      | None                                                                                                                              | [GetBeersQuery](./Services/BeerManagement/src/Application/Beers/Queries/GetBeers/GetBeersQuery.cs) | Gets all beers                                     | PaginatedList<[BeerDto](./Services/BeerManagement/src/Application/Beers/Dtos/BeerDto.cs)> | Everyone       |
| GET    | /api/Beers/{id} | None                                                                                                                              | Id:GUID                                                                                            | Gets beer by id                                    | [BeerDto](./Services/BeerManagement/src/Application/Beers/Dtos/BeerDto.cs), 404           | Everyone       |
| POST   | /api/Beers      | [CreateBeerCommand](./Services/BeerManagement/src/Application/Beers/Commands/CreateBeer/CreateBeerCommand.cs)                     | None                                                                                               | Creates the beer                                   | 201, 400, 404, 401, 403                                                                   | Administrator  |
| PUT    | /api/Beers/{id} | [UpdateBeerCommand](./Services/BeerManagement/src/Application/Beers/Commands/UpdateBeer/UpdateBeerCommand.cs)                     | Id:GUID                                                                                            | Updates the beer                                   | 204, 400, 404, 401, 403                                                                   | Administrator  |
| DELETE | /api/Beers/{id} | [DeleteBeerCommand](./Services/BeerManagement/src/Application/Beers/Commands/DeleteBeer/DeleteBeerCommand.cs)                     | None                                                                                               | Deletes the beer                                   | 204, 400, 404, 401, 403                                                                   | Administrator  |
| POST   | /api/Beers/{id} | [UpsertBeerImageCommand](./Services/BeerManagement/src/Application/BeerImages/Commands/UpsertBeerImage/UpsertBeerImageCommand.cs) | Id:GUID                                                                                            | Creates or updates the beer image                  | 201, 400, 404, 401, 403                                                                   | Administrator  |
| DELETE | /api/Beers/{id} | None                                                                                                                              | Id:GUID                                                                                            | Deletes the beer image and restores the temp image | 204, 400, 404, 401, 403                                                                   | Administrator  |

#### BeerStyles Controller

| Method | Path                 | Body                                                                                                                              | Params                                                                                                                 | Description            | Responses                                                                                                | Who can access |
| :----- | :------------------- | :-------------------------------------------------------------------------------------------------------------------------------- | :--------------------------------------------------------------------------------------------------------------------- | :--------------------- | :------------------------------------------------------------------------------------------------------- | :------------- |
| GET    | /api/BeerStyles      | None                                                                                                                              | [GetBeerStylesQuery](./Services/BeerManagement/src/Application/BeerStyles/Queries/GetBeerStyles/GetBeerStylesQuery.cs) | Gets all beer styles   | PaginatedList<[BeerStyleDto](./Services/BeerManagement/src/Application/BeerStyles/Dtos/BeerStyleDto.cs)> | Everyone       |
| GET    | /api/BeerStyles/{id} | None                                                                                                                              | Id:GUID                                                                                                                | Gets beer style by id  | [BeerStyleDto](./Services/BeerManagement/src/Application/BeerStyles/Dtos/BeerStyleDto.cs), 404           | Everyone       |
| POST   | /api/BeerStyles      | [CreateBeerStyleCommand](./Services/BeerManagement/src/Application/BeerStyles/Commands/CreateBeerStyle/CreateBeerStyleCommand.cs) | None                                                                                                                   | Creates the beer style | 201, 400, 404, 401, 403                                                                                  | Administrator  |
| PUT    | /api/BeerStyles/{id} | [UpdateBeerStyleCommand](./Services/BeerManagement/src/Application/BeerStyles/Commands/UpdateBeerStyle/UpdateBeerStyleCommand.cs) | Id:GUID                                                                                                                | Updates the beer style | 204, 400, 404, 401, 403                                                                                  | Administrator  |
| DELETE | /api/BeerStyles/{id} | [DeleteBeerStyleCommand](./Services/BeerManagement/src/Application/BeerStyles/Commands/DeleteBeerStyle/DeleteBeerStyleCommand.cs) | None                                                                                                                   | Deletes the beer style | 204, 400, 404, 401, 403                                                                                  | Administrator  |

#### Breweries Controller

| Method | Path                | Body                                                                                                                       | Params                                                                                                             | Description         | Responses                                                                                           | Who can access |
| :----- | :------------------ | :------------------------------------------------------------------------------------------------------------------------- | :----------------------------------------------------------------------------------------------------------------- | :------------------ | :-------------------------------------------------------------------------------------------------- | :------------- |
| GET    | /api/Breweries      | None                                                                                                                       | [GetBreweriesQuery](./Services/BeerManagement/src/Application/Breweries/Queries/GetBreweries/GetBreweriesQuery.cs) | Gets all breweries  | PaginatedList<[BreweryDto](./Services/BeerManagement/src/Application/Breweries/Dtos/BreweryDto.cs)> | Everyone       |
| GET    | /api/Breweries/{id} | None                                                                                                                       | Id:GUID                                                                                                            | Gets brewery by id  | [BreweryDto](./Services/BeerManagement/src/Application/Breweries/Dtos/BreweryDto.cs), 404           | Everyone       |
| POST   | /api/Breweries      | [CreateBreweryCommand](./Services/BeerManagement/src/Application/Breweries/Commands/CreateBrewery/CreateBreweryCommand.cs) | None                                                                                                               | Creates the brewery | 201, 400, 404, 401, 403                                                                             | Administrator  |
| PUT    | /api/Breweries/{id} | [UpdateBreweryCommand](./Services/BeerManagement/src/Application/Breweries/Commands/UpdateBrewery/UpdateBreweryCommand.cs) | Id:GUID                                                                                                            | Updates the brewery | 204, 400, 404, 401, 403                                                                             | Administrator  |
| DELETE | /api/Breweries/{id} | [DeleteBreweryCommand](./Services/BeerManagement/src/Application/Breweries/Commands/DeleteBrewery/DeleteBreweryCommand.cs) | None                                                                                                               | Deletes the brewery | 204, 400, 404, 401, 403                                                                             | Administrator  |

### OpinionManagement Service

#### Opinions Controller

### FavoriteManagement Service

#### Favorites Controller

## Tests

This project uses unit tests provided by xUnit.

To run tests go to the root solution directory and do:

`dotnet test`

## License

[MIT](https://choosealicense.com/licenses/mit/)
