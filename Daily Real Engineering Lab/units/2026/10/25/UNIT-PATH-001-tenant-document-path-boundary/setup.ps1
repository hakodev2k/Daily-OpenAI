$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot
& dotnet --version
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& dotnet restore ./baseline/PathBoundaryLab.Baseline.csproj
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& dotnet restore ./starter/PathBoundaryLab.csproj
exit $LASTEXITCODE
