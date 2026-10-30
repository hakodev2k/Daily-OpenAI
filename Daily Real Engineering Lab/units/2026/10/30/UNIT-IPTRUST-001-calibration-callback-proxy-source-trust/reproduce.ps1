$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./Lab.csproj -- reproduce
    exit $LASTEXITCODE
} finally { Pop-Location }
