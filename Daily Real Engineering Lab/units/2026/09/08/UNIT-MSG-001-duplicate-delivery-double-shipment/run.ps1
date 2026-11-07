$ErrorActionPreference = "Stop"
$work = Join-Path $PSScriptRoot ".work/Runner"
if (Test-Path $work) { Remove-Item $work -Recurse -Force }
dotnet new console --framework net8.0 --output $work --force | Out-Null
Copy-Item (Join-Path $PSScriptRoot "starter/Program.cs") (Join-Path $work "Program.cs") -Force
dotnet run --project (Join-Path $work "Runner.csproj")
