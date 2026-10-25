$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot
& dotnet run --project ./starter/PathBoundaryLab.csproj -- --verify
exit $LASTEXITCODE
