$ErrorActionPreference = 'Stop'
$project = './starter/Lab/Lab.csproj'

dotnet build $project
$process = Start-Process dotnet -ArgumentList @('run','--no-build','--project',$project) -NoNewWindow -PassThru -Wait

if ($process.ExitCode -eq 1) {
    Write-Host 'REPRODUCED: starter exposes divergent singleton state across provider graphs.'
    exit 0
}

throw "Expected starter to expose the symptom with exit code 1, but exit code was $($process.ExitCode)."
