$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot
& dotnet run --project ./starter/PathBoundaryLab.csproj -- --inspect
exit $LASTEXITCODE
