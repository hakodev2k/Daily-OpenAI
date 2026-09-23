$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet run --project "$root/starter/SecretRotationLab.csproj"
Write-Host "Compare the store current version with the credential observed after rotation."