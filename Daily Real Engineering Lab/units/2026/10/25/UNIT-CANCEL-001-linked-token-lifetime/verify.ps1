param([switch]$Reference)
$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    $project = './probe/ScannerLab.Probe.csproj'
    if ($Reference) { $project = './solution/ScannerLab.Reference.csproj' }
    & dotnet run --project $project -- verify
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
finally { Pop-Location }
