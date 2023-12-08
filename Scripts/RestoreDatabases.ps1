param(
    [string]$server = "localhost\SQLEXPRESS",
    [string]$user = "",
    [string]$password = ""
)

# Get the current directory
$initialDirectory = Convert-Path -Path "..\"

function Restore-Database
{
    param (
        [string]$solution
    )

    # Get connection string
    $connectionString = "Server=$server;Database=$solution;User=$user;Password=$password;Trusted_Connection=True;TrustServerCertificate=True"

    # Extract the directory path
    $solutionDirectory = Join-Path -Path (Join-Path -Path $initialDirectory -ChildPath "Services") -ChildPath "$solution"

    # Change to the solution directory
    Set-Location -Path $solutionDirectory

    # Clean the database (drop and recreate)
    dotnet ef database update 0 --connection $connectionString --project src/api
    dotnet ef database update --connection $connectionString --project src/api --no-build
}

function Initialize-Database
{
    param (
        [string]$scriptName,
        [string]$databaseName
    )

    $scriptPath = Join-Path -Path $initialDirectory -ChildPath "Scripts\SQLScripts\$scriptName.sql"
    Write-Host "Seeding $scriptName..." -ForegroundColor Blue
    Invoke-Sqlcmd -InputFile $scriptPath -ServerInstance $server -Database $databaseName
}

function Restore-BeerManagement
{
    $solution = "BeerManagement"
    Write-Host "Restoring database for $solution" -ForegroundColor DarkMagenta
    Restore-Database -solution $solution
    Initialize-Database -scriptName "BeerManagement_Breweries" -databaseName $solution
    Initialize-Database -scriptName "BeerManagement_BeerStyles" -databaseName $solution
    Initialize-Database -scriptName "BeerManagement_Beers" -databaseName $solution
    Write-Host "$solution restored successfully" -ForegroundColor Green
}

function Restore-UserManagement
{
    $solution = "UserManagement"
    Write-Host "Restoring database for $solution" -ForegroundColor DarkMagenta
    Restore-Database -solution $solution
    Initialize-Database -scriptName "UserManagement_Users" -databaseName $solution
    Write-Host "$solution restored successfully" -ForegroundColor Green
}

function Restore-OpinionManagement
{
    $solution = "OpinionManagement"
    Write-Host "Restoring database for $solution" -ForegroundColor DarkMagenta
    Restore-Database -solution $solution
    Initialize-Database -scriptName "OpinionManagement_Users" -databaseName $solution
    Initialize-Database -scriptName "OpinionManagement_Beers" -databaseName $solution
    Initialize-Database -scriptName "OpinionManagement_Opinions" -databaseName $solution
    Write-Host "$solution restored successfully" -ForegroundColor Green
}

function Restore-FavoriteManagement
{
    $solution = "FavoriteManagement"
    Write-Host "Restoring database for $solution" -ForegroundColor DarkMagenta
    Restore-Database -solution $solution
    Initialize-Database -scriptName "FavoriteManagement_Users" -databaseName $solution
    Initialize-Database -scriptName "FavoriteManagement_Beers" -databaseName $solution
    Initialize-Database -scriptName "FavoriteManagement_Favorites" -databaseName $solution
    Write-Host "$solution restored successfully" -ForegroundColor Green
}

# Restore databases
Restore-BeerManagement
Restore-UserManagement
Restore-OpinionManagement
Restore-FavoriteManagement

Set-Location -Path "$initialDirectory/Scripts"