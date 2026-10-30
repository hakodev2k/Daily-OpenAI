$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./Lab.csproj -- verify
    exit $LASTEXITCODE
} finally { Pop-Location }
