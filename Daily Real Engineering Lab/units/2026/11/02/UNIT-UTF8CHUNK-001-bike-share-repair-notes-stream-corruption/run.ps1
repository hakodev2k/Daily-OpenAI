$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/Utf8ChunkLab.csproj -- --run
    if ($LASTEXITCODE -ne 0) { throw "run failed (exit code $LASTEXITCODE)." }
} finally { Pop-Location }
