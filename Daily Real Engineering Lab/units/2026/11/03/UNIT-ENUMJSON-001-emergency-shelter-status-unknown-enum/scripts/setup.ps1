$ErrorActionPreference="Stop"
dotnet restore "$PSScriptRoot/../starter/ShelterFeed.csproj"
if($LASTEXITCODE -ne 0){throw "Restore failed"}
