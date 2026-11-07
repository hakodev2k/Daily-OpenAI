$ErrorActionPreference = 'Stop'
$unit = Split-Path -Parent $PSScriptRoot
dotnet --version
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet restore (Join-Path $unit 'harness/Probe.csproj')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet restore (Join-Path $unit 'solution/SolutionProbe.csproj')
exit $LASTEXITCODE
