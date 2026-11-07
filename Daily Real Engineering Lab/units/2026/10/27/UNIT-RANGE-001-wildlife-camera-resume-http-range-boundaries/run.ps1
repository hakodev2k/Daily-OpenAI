$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/RangeLab/RangeLab.csproj -- --observe
    if ($LASTEXITCODE -ne 0) { throw "Runner failed: $LASTEXITCODE" }
} finally { Pop-Location }
