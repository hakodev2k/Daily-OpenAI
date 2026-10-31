$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { throw '.NET SDK 8.0.x is required.' }
    & dotnet restore './starter/CallbackGate.csproj'
    if ($LASTEXITCODE -ne 0) { throw 'Starter restore failed.' }
    & dotnet restore './verification/Verification.csproj'
    if ($LASTEXITCODE -ne 0) { throw 'Verification restore failed.' }
    Write-Host 'Setup complete. No external NuGet packages.'
} finally { Pop-Location }
