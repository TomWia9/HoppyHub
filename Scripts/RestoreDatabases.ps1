param(
    [string]$Server = "localhost\SQLEXPRESS"
)

# List of solutions
$solutions = @(
    "BeerManagement",
    "FavoriteManagement",
    "OpinionManagement",
    "UserManagement"
)

# Get the current directory
$initialDirectory = Convert-Path -Path "..\"

# Loop through each solution
foreach ($solution in $solutions) {
    Write-Host "Restoring database for solution: $solution" -ForegroundColor Blue
    
    # Extract the directory path
    $solutionDirectory = Join-Path -Path (Join-Path -Path $initialDirectory -ChildPath "Services") -ChildPath "$solution"

    # Change to the solution directory
    Set-Location -Path $solutionDirectory
    
    # Clean the database (drop and recreate)
    dotnet ef database drop --force --project src/api
    dotnet ef database update --project src/api

    # Return to the initial directory
    Set-Location -Path $initialDirectory

    Write-Host "Success" -ForegroundColor Green
}

Write-Host "Seeding BeerManagement database" -ForegroundColor DarkBlue
Write-Host "Seeding Breweries..." -ForegroundColor Blue

# Execute the SQL script to seed the database
$scriptPath = Join-Path -Path $initialDirectory -ChildPath "Scripts\SQLScripts\Breweries.sql"
Invoke-Sqlcmd -InputFile $scriptPath -ServerInstance $Server -Database "BeerManagement"

Write-Host "Breweries seeded successfully" -ForegroundColor Green

Write-Host "Seeding Beer styles..." -ForegroundColor Blue

# Execute the SQL script to seed the database
$scriptPath = Join-Path -Path $initialDirectory -ChildPath "Scripts\SQLScripts\BeerStyles.sql"
Invoke-Sqlcmd -InputFile $scriptPath -ServerInstance $Server -Database "BeerManagement"

Write-Host "Beer styles seeded successfully" -ForegroundColor Green

Write-Host "Seeding Beers..." -ForegroundColor Blue

# Execute the SQL script to seed the database
$scriptPath = Join-Path -Path $initialDirectory -ChildPath "Scripts\SQLScripts\Beers.sql"
Invoke-Sqlcmd -InputFile $scriptPath -ServerInstance $Server -Database "BeerManagement"

Write-Host "Beers seeded successfully" -ForegroundColor Green