$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $root 'starter/SharedStateLab.csproj'

Write-Host 'Restore + build learner-editable starter'
dotnet restore $project
dotnet build $project --no-restore
if ($LASTEXITCODE -ne 0) { throw 'Starter does not build.' }

Write-Host 'Verify behavior while both tests execute through the parallel gate'
$env:LAB_PARALLEL_GATE = '1'
dotnet test $project --no-build
$exitCode = $LASTEXITCODE
Remove-Item Env:LAB_PARALLEL_GATE -ErrorAction SilentlyContinue

if ($exitCode -ne 0) {
    throw 'Verification failed. The learner-editable starter still has cross-test interference or a behavior regression.'
}

Write-Host 'Verification passed: both behaviors remain correct under parallel execution.'
