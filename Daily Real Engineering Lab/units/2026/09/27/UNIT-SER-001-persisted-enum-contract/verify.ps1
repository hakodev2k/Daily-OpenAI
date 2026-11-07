$ErrorActionPreference = 'Stop'

dotnet run --project ./starter/SerializationLab.csproj
$code = $LASTEXITCODE

if ($code -ne 0) {
    throw 'Verification failed: learner-editable starter still violates the historical serialization contract.'
}

Write-Host 'Verification passed: historical payload keeps the expected business meaning.'
