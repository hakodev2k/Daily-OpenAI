$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet run --project "$root/starter/TimerOverlapLab.csproj" -- verify
if ($LASTEXITCODE -ne 0) { throw "Learner starter does not yet satisfy the concurrency and shutdown contract." }
Write-Host "VERIFY=PASS"
