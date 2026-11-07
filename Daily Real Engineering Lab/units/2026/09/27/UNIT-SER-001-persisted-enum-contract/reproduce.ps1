$ErrorActionPreference = 'Stop'

dotnet run --project ./starter/SerializationLab.csproj
$code = $LASTEXITCODE

if ($code -eq 0) {
    throw 'Expected the original starter to reproduce the historical contract mismatch, but it passed.'
}

Write-Host 'Reproduction confirmed: historical payload is interpreted inconsistently.'
exit 0
