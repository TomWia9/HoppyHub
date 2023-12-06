# List of solutions with databases

$solutionsWithDatabases = @(
    "../Services/BeerManagement/BeerManagement.sln",
    "../Services/FavoriteManagement/FavoriteManagement.sln",
    "../Services/OpinionManagement/OpinionManagement.sln",
    "../Services/UserManagement/UserManagement.sln"
)

# Get the current directory
$initialDirectory = Get-Location

# Loop through each solution
foreach ($solution in $solutionsWithDatabases) {
    Write-Host "Restoring database for solution: $solution" -ForegroundColor Green
    
    # Extract the directory path
    $solutionDirectory = Split-Path -Path $solution -Parent

    # Change to the solution directory
    Set-Location -Path $solutionDirectory
    
    # Execute dotnet ef database update
    dotnet ef database update --project src/api
    
    # Return to the initial directory
    Set-Location -Path $initialDirectory

    Write-Host "Success" -ForegroundColor Green
}
