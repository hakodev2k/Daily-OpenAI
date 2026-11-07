$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'SharedStateLab.csproj'
$env:LAB_PARALLEL_GATE = '1'
dotnet test $project
$exitCode = $LASTEXITCODE
Remove-Item Env:LAB_PARALLEL_GATE -ErrorAction SilentlyContinue
if ($exitCode -ne 0) { throw 'Reference solution verification failed.' }
Write-Host 'Reference solution passed under parallel execution.'
