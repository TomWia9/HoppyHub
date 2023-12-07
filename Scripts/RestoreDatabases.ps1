param(
    [string]$Server = "localhost\SQLEXPRESS"
)

# Get the current directory
$initialDirectory = Convert-Path -Path "..\"

function Restore-Database {
    param (
        [string]$solution
    )

    # Extract the directory path
    $solutionDirectory = Join-Path -Path (Join-Path -Path $initialDirectory -ChildPath "Services") -ChildPath "$solution"

    # Change to the solution directory
    Set-Location -Path $solutionDirectory

    # Clean the database (drop and recreate)
    dotnet ef database drop --force --project src/api
    dotnet ef database update --project src/api

    Write-Host "Success" -ForegroundColor Green
}

function Initialize-Database {
    param (
        [string]$name,
        [string]$database
    )

    $scriptPath = Join-Path -Path $initialDirectory -ChildPath "Scripts\SQLScripts\$name.sql"
    Write-Host "Seeding $name..." -ForegroundColor Blue
    Invoke-Sqlcmd -InputFile $scriptPath -ServerInstance $Server -Database "BeerManagement"
}

function Restore-BeerManagement {
    $solution = "BeerManagement"
    Write-Host "Restoring database for $solution" -ForegroundColor DarkMagenta
    Restore-Database -solution $solution
    Write-Host "Seeding $solution..." -ForegroundColor DarkMagenta
    Initialize-Database -name "Breweries" -Database "BeerManagement"
    Initialize-Database -name "BeerStyles" -Database "BeerManagement"
    Initialize-Database -name "Beers" -Database "BeerManagement"
    Write-Host "$solution restored successfully" -ForegroundColor Green
}

Restore-BeerManagement

Set-Location -Path $initialDirectory