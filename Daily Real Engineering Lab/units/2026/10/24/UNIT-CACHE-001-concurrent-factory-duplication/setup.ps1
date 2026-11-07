$ErrorActionPreference = 'Stop'
foreach ($relative in @('baseline/CacheFactoryLab.Baseline.csproj', 'starter/CacheFactoryLab.csproj', 'solution/CacheFactoryLab.Solution.csproj')) {
    dotnet build (Join-Path $PSScriptRoot $relative) --nologo
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
