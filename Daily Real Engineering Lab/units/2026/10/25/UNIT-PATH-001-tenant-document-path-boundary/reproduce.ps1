$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot
& dotnet run --project ./baseline/PathBoundaryLab.Baseline.csproj -- --reproduce
exit $LASTEXITCODE
