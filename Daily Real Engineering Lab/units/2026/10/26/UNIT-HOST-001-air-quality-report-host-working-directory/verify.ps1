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
    $expected = 'Station NorthStation: AQI 42.'
    foreach ($case in @(@{ Name='interactive'; Result=$interactive }, @{ Name='scheduled'; Result=$scheduled })) {
        if ($case.Result.ExitCode -ne 0) { throw "$($case.Name) exit code: $($case.Result.ExitCode)" }
        if ($case.Result.Output -ne $expected) { throw "$($case.Name) output mismatch: [$($case.Result.Output)]" }
        Write-Host "PASS $($case.Name): $($case.Result.Output)"
    }
    Write-Host 'VERIFIED: learner-edited starter works from both launch contexts.'
} finally { Remove-Item $scratch -Recurse -Force -ErrorAction SilentlyContinue }
