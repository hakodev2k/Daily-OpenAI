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
$result = Invoke-Report $projectDir $dll
Write-Output $result.Output
if ($result.ExitCode -ne 0) { throw "Worker exited with code $($result.ExitCode)" }
