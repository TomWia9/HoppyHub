param(
    [string]$Server = "localhost\SQLEXPRESS"
)

# Get the current directory
$initialDirectory = Convert-Path -Path "..\"

function Restore-Database {
    param (
        [string]$solution
    )

    # Get connection string
    $connectionString = "Server=$server;Database=$solution;Trusted_Connection=True;TrustServerCertificate=True"

    # Extract the directory path
    $solutionDirectory = Join-Path -Path (Join-Path -Path $initialDirectory -ChildPath "Services") -ChildPath "$solution"

    # Change to the solution directory
    Set-Location -Path $solutionDirectory

    # Clean the database (drop and recreate)
    dotnet ef database update 0 --connection $connectionString --project src/api
    dotnet ef database update --connection $connectionString --project src/api --no-build

    Write-Host "Success" -ForegroundColor Green
}

function Initialize-Database {
    param (
        [string]$name,
        [string]$database
    )

    $scriptPath = Join-Path -Path $initialDirectory -ChildPath "Scripts\SQLScripts\$name.sql"
    Write-Host "Seeding $name..." -ForegroundColor Blue
    Invoke-Sqlcmd -InputFile $scriptPath -ServerInstance $Server -Database $database
}

function Restore-BeerManagement {
    $solution = "BeerManagement"
    Write-Host "Restoring database for $solution" -ForegroundColor DarkMagenta
    Restore-Database -solution $solution
    Write-Host "Seeding $solution..." -ForegroundColor DarkMagenta
    Initialize-Database -name "Breweries" -Database $solution
    Initialize-Database -name "BeerStyles" -Database $solution
    Initialize-Database -name "Beers" -Database $solution
    Write-Host "$solution restored successfully" -ForegroundColor Green
}

function Restore-UserManagement {
    $solution = "UserManagement"
    Write-Host "Restoring database for $solution" -ForegroundColor DarkMagenta
    Restore-Database -solution $solution
    Write-Host "Seeding $solution..." -ForegroundColor DarkMagenta
    Initialize-Database -name "Users" -Database $solution
    Write-Host "$solution restored successfully" -ForegroundColor Green
}

function Restore-OpinionManagement {
    $solution = "OpinionManagement"
    Write-Host "Restoring database for $solution" -ForegroundColor DarkMagenta
    Restore-Database -solution $solution
    # Write-Host "Seeding $solution..." -ForegroundColor DarkMagenta
    # Initialize-Database -name "Users" -Database $solution
    # Initialize-Database -name "Beers" -Database $solution
    # Initialize-Database -name "Opinions" -Database $solution
    Write-Host "$solution restored successfully" -ForegroundColor Green
}

function Restore-FavoriteManagement {
    $solution = "FavoriteManagement"
    Write-Host "Restoring database for $solution" -ForegroundColor DarkMagenta
    Restore-Database -solution $solution
    # Write-Host "Seeding $solution..." -ForegroundColor DarkMagenta
    # Initialize-Database -name "Users" -Database $solution
    # Initialize-Database -name "Beers" -Database $solution
    # Initialize-Database -name "Favorites" -Database $solution
    Write-Host "$solution restored successfully" -ForegroundColor Green
}

# Restore databases
Restore-BeerManagement
Restore-UserManagement
Restore-OpinionManagement
Restore-FavoriteManagement

Set-Location -Path "$initialDirectory/Scripts"