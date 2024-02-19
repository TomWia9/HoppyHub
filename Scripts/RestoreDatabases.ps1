# Set default parameters for the server, username, and password. 
# If not provided, it assumes a local SQL Server instance.
param(
    [string]$server = "localhost\SQLEXPRESS",
    [string]$user = "",
    [string]$password = "",
    [switch]$docker
)

# Get the current directory and navigate back one level.
$initialDirectory = Convert-Path -Path "..\"

if ($docker) {
	$server = "localhost,1433"
    $user = "sa"
    $password = "HoppyHub123!"
}

# A function to restore a specified database.
function Restore-Database
{
    param (
        [string]$databaseName
    )

    # Construct a connection strings using provided or default parameters.
    $masterConnectionString = "Server=$server;User=$user;Password=$password;Integrated Security=True;Initial Catalog=master;TrustServerCertificate=True"
    $connectionString = "Server=$server;Database=$databaseName;User=$user;Password=$password;Trusted_Connection=True;TrustServerCertificate=True"

    if ($docker) {
        $masterConnectionString = "Server=$server;User=$user;Password=$password;Initial Catalog=master;Encrypt=False;"
        $connectionString = "Server=$server;Database=$databaseName;User=$user;Password=$password;Encrypt=False;"
    }

    # Create a path for the directory of the specific solution's.
    $solutionDirectory = Join-Path -Path (Join-Path -Path $initialDirectory -ChildPath "Services") -ChildPath "$databaseName"

    # Change the current directory to the solution's directory.
    Set-Location -Path $solutionDirectory

    # Check if database exists
    $query = "SELECT COUNT(*) FROM sys.databases WHERE name = '$databaseName'"
    $result = Invoke-Sqlcmd -ConnectionString $masterConnectionString -Query $query

    # Drop database if exists
    if ($result.ItemArray[0] -eq 1) {
        Invoke-Sqlcmd -ConnectionString $masterConnectionString -Query "ALTER DATABASE $databaseName SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE $databaseName;"
    }

    # Update database schema
    dotnet ef database update --connection $connectionString --project src/api > $null
}

# A function to initialize a database using SQL scripts.
function Initialize-Database
{
    param (
        [string]$scriptName,
        [string]$databaseName
    )

    # Construct a connection string using provided or default parameters.
    $connectionString = "Server=$server;Database=$databaseName;User=$user;Password=$password;Trusted_Connection=True;TrustServerCertificate=True"

    if ($docker) {
        $connectionString = "Server=$server;Database=$databaseName;User=$user;Password=$password;Encrypt=False;"
    }

    # Execute SQL scripts to initialize the database.
    $scriptPath = Join-Path -Path $initialDirectory -ChildPath "Scripts\SQLScripts\$scriptName.sql"
    Write-Host "Seeding $scriptName..."
    Invoke-Sqlcmd -InputFile $scriptPath -ConnectionString $connectionString
}

# Functions for specific databases to be restored and initialized:

# Restoring and initializing the BeerManagement database.
function Restore-BeerManagement {
    $databaseName = "BeerManagement"
    Write-Host "Restoring $databaseName" -ForegroundColor DarkMagenta
    Restore-Database -databaseName $databaseName
    Initialize-Database -scriptName "BeerManagement_Breweries" -databaseName $databaseName
    Initialize-Database -scriptName "BeerManagement_BeerStyles" -databaseName $databaseName
    Initialize-Database -scriptName "BeerManagement_Beers" -databaseName $databaseName
    Write-Host "$databaseName restored successfully" -ForegroundColor Green
}

# Restoring and initializing the UserManagement database.
function Restore-UserManagement {
    $databaseName = "UserManagement"
    Write-Host "Restoring $databaseName" -ForegroundColor DarkMagenta
    Restore-Database -databaseName $databaseName
    Initialize-Database -scriptName "UserManagement_Users" -databaseName $databaseName
    Write-Host "$databaseName restored successfully" -ForegroundColor Green
}

# Restoring and initializing the OpinionManagement database.
function Restore-OpinionManagement {
    $databaseName = "OpinionManagement"
    Write-Host "Restoring $databaseName" -ForegroundColor DarkMagenta
    Restore-Database -databaseName $databaseName
    Initialize-Database -scriptName "OpinionManagement_Users" -databaseName $databaseName
    Initialize-Database -scriptName "OpinionManagement_Beers" -databaseName $databaseName
    Initialize-Database -scriptName "OpinionManagement_Opinions" -databaseName $databaseName
    Write-Host "$databaseName restored successfully" -ForegroundColor Green
}

# Restoring and initializing the FavoriteManagement database.
function Restore-FavoriteManagement {
    $databaseName = "FavoriteManagement"
    Write-Host "Restoring $databaseName" -ForegroundColor DarkMagenta
    Restore-Database -databaseName $databaseName
    Initialize-Database -scriptName "FavoriteManagement_Users" -databaseName $databaseName
    Initialize-Database -scriptName "FavoriteManagement_Beers" -databaseName $databaseName
    Initialize-Database -scriptName "FavoriteManagement_Favorites" -databaseName $databaseName
    Write-Host "$databaseName restored successfully" -ForegroundColor Green
}

# Perform database restoration and initialization for different databases:
Restore-BeerManagement
Restore-UserManagement
Restore-OpinionManagement
Restore-FavoriteManagement

Write-Host "ALL DATABASES RESTORED SUCCESSFULLY" -ForegroundColor Green

# After restoring all databases, navigate back to the initial script directory.
Set-Location -Path "$initialDirectory/Scripts"