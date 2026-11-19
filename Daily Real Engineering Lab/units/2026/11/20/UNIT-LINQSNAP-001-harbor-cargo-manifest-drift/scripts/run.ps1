$ErrorActionPreference = "Stop"
dotnet run --project (Join-Path $PSScriptRoot "../starter/CargoManifestLab.csproj")
if ($LASTEXITCODE -ne 0) { throw "Program failed." }
