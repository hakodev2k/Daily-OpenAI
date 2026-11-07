$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    & dotnet run --project './starter/CallbackGate.csproj' --configuration Release
    $code = $LASTEXITCODE
    if ($code -ne 2) { throw "Expected original starter exit 2; got $code. Reproduce before editing." }
    Write-Host 'REPRODUCED: original starter violates callback policy; this is not fix verification.'
} finally { Pop-Location }
