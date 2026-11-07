$ErrorActionPreference = 'Stop'
$project = './starter/Lab/Lab.csproj'

dotnet build $project
dotnet run --no-build --project $project

if ($LASTEXITCODE -ne 0) {
    throw 'Verification failed: learner-editable starter still exposes divergent state.'
}

Write-Host 'VERIFIED: learner-editable starter uses one consistent singleton graph.'
