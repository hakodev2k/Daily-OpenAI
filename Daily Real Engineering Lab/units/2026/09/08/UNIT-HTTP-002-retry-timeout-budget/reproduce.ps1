$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"
dotnet run --project $project
if ($LASTEXITCODE -ne 0) { exit 1 }
Write-Host "Observe that three 300 ms attempts make the total operation exceed the 500 ms budget."
