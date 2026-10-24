$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    & dotnet run --project './probe/ScannerLab.Probe.csproj' -- observe
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
finally { Pop-Location }
