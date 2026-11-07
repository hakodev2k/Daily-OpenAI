$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./Lab.csproj -- inspect
    exit $LASTEXITCODE
} finally { Pop-Location }
