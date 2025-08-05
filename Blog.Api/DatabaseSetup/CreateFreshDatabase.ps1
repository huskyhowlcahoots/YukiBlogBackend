# Move one level up
Set-Location ..


$folderPath = ".\.db"

# Check if the folder exists
if (Test-Path $folderPath) {
    # Then nuke the folder along with it's DB content
    Remove-Item -Recurse -Force $folderPath
}

# Create the new folder
New-Item -ItemType Directory -Path $folderPath

# Apply latest migration scripts to the create a new DB for us
dotnet ef database update
