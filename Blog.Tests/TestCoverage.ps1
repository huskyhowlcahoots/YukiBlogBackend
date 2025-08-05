# Navigate into the test project directory
Set-Location -Path "$PWD"

# Run tests with specified settings and collect coverage
dotnet test --settings "coverlet.runsettings" --collect:"XPlat Code Coverage"

# Find the latest coverage.cobertura.xml
$latestCoverage = Get-ChildItem -Path ".\TestResults\" -Recurse -Filter "coverage.cobertura.xml" |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1

if (-not $latestCoverage) {
    Write-Error "Could not find coverage.cobertura.xml"
    exit 1
}

# Generate HTML report
reportgenerator -reports:$latestCoverage.FullName -targetdir:"coveragereport" -reporttypes:"Html"

# Open the HTML report
Start-Process "coveragereport\index.html"
