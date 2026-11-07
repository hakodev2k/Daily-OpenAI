$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/Utf8ChunkLab.csproj -- --verify
    if ($LASTEXITCODE -ne 0) { throw "Learner code failed the decoding contract (exit code $LASTEXITCODE)." }
} finally { Pop-Location }
