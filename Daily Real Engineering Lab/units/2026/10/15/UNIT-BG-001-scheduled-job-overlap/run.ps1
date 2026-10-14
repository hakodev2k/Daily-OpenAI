$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet run --project "$root/starter/TimerOverlapLab.csproj" -- verify
if ($LASTEXITCODE -ne 0) { throw "Starter run failed its behavioral checks." }
