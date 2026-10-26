$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ReportWorker/ReportWorker.csproj'
$projectDir = Join-Path $PSScriptRoot 'starter/ReportWorker'
$dll = Join-Path $PSScriptRoot 'starter/ReportWorker/bin/Release/net8.0/ReportWorker.dll'
& dotnet build $project -c Release --nologo
if ($LASTEXITCODE -ne 0) { throw 'Build failed. Install .NET 8 SDK.' }
function Invoke-Report([string]$workingDir, [string]$assembly) {
    Push-Location $workingDir
    try {
        $output = (& dotnet $assembly | Out-String).Trim()
        $code = $LASTEXITCODE
        return @{ ExitCode = $code; Output = $output }
    } finally { Pop-Location }
}
$scratch = Join-Path ([System.IO.Path]::GetTempPath()) ("air-quality-lab-" + [guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Path $scratch | Out-Null
try {
    $interactive = Invoke-Report $projectDir $dll
    $scheduled = Invoke-Report $scratch $dll
    Write-Host "INTERACTIVE exit=$($interactive.ExitCode) stdout=$($interactive.Output)"
    Write-Host "SCHEDULED exit=$($scheduled.ExitCode) stdout=$($scheduled.Output)"
    if ($interactive.ExitCode -ne 0 -or $interactive.Output -ne 'Station NorthStation: AQI 42.') { throw 'Control run failed.' }
    if ($scheduled.ExitCode -eq 0) { throw 'Original failure not reproduced.' }
    Write-Host 'REPRODUCED: same assembly succeeds interactively and fails in scheduled launch context.'
} finally { Remove-Item $scratch -Recurse -Force -ErrorAction SilentlyContinue }
