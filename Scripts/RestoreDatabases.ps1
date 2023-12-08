# Set default parameters for the server, username, and password. 
# If not provided, it assumes a local SQL Server instance.
param(
    [string]$server = "localhost\SQLEXPRESS",
    [string]$user = "",
    [string]$password = ""
)

# Get the current directory and navigate back one level.
$initialDirectory = Convert-Path -Path "..\"

# A function to restore a specified database.
function Restore-Database
{
    param (
        [string]$solution
    )

    # Construct a connection string using provided or default parameters.
    $connectionString = "Server=$server;Database=$solution;User=$user;Password=$password;Trusted_Connection=True;TrustServerCertificate=True"

    # Create a path for the directory of the specific solution's.
    $solutionDirectory = Join-Path -Path (Join-Path -Path $initialDirectory -ChildPath "Services") -ChildPath "$solution"

    # Change the current directory to the solution's directory.
    Set-Location -Path $solutionDirectory

    # Execute Entity Framework commands to update the database schema.
    dotnet ef database update 0 --connection $connectionString --project src/api > $null
    dotnet ef database update --connection $connectionString --project src/api --no-build > $null

}

# A function to initialize a database using SQL scripts.
function Initialize-Database
{
    param (
        [string]$scriptName,
        [string]$databaseName
    )

    # Execute SQL scripts to initialize the database.
    $scriptPath = Join-Path -Path $initialDirectory -ChildPath "Scripts\SQLScripts\$scriptName.sql"
    Write-Host "Seeding $scriptName..." 
    Invoke-Sqlcmd -InputFile $scriptPath -ServerInstance $server -Database $databaseName

}

# Functions for specific databases to be restored and initialized:

# Restoring and initializing the BeerManagement database.
function Restore-BeerManagement {
    $solution = "BeerManagement"
    Write-Host "Restoring database for $solution" -ForegroundColor DarkMagenta
    Restore-Database -solution $solution
    Initialize-Database -scriptName "BeerManagement_Breweries" -databaseName $solution
    Initialize-Database -scriptName "BeerManagement_BeerStyles" -databaseName $solution
    Initialize-Database -scriptName "BeerManagement_Beers" -databaseName $solution
    Write-Host "$solution restored successfully" -ForegroundColor Green
}

# Restoring and initializing the UserManagement database.
function Restore-UserManagement {
    $solution = "UserManagement"
    Write-Host "Restoring database for $solution" -ForegroundColor DarkMagenta
    Restore-Database -solution $solution
    Initialize-Database -scriptName "UserManagement_Users" -databaseName $solution
    Write-Host "$solution restored successfully" -ForegroundColor Green
}

# Restoring and initializing the OpinionManagement database.
function Restore-OpinionManagement {
    $solution = "OpinionManagement"
    Write-Host "Restoring database for $solution" -ForegroundColor DarkMagenta
    Restore-Database -solution $solution
    Initialize-Database -scriptName "OpinionManagement_Users" -databaseName $solution
    Initialize-Database -scriptName "OpinionManagement_Beers" -databaseName $solution
    Initialize-Database -scriptName "OpinionManagement_Opinions" -databaseName $solution
    Write-Host "$solution restored successfully" -ForegroundColor Green
}

# Restoring and initializing the FavoriteManagement database.
function Restore-FavoriteManagement {
    $solution = "FavoriteManagement"
    Write-Host "Restoring database for $solution" -ForegroundColor DarkMagenta
    Restore-Database -solution $solution
    Initialize-Database -scriptName "FavoriteManagement_Users" -databaseName $solution
    Initialize-Database -scriptName "FavoriteManagement_Beers" -databaseName $solution
    Initialize-Database -scriptName "FavoriteManagement_Favorites" -databaseName $solution
    Write-Host "$solution restored successfully" -ForegroundColor Green
}

# Perform database restoration and initialization for different databases:
Restore-BeerManagement
Restore-UserManagement
Restore-OpinionManagement
Restore-FavoriteManagement

Write-Host "ALL DATABASES RESTORED SUCCESSFULLY" -ForegroundColor Green

# After restoring all databases, navigate back to the initial script directory.
Set-Location -Path "$initialDirectory/Scripts"
