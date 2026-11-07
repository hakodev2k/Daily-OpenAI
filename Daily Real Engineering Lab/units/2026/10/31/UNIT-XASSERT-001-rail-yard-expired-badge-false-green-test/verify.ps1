$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    Remove-Item Env:LAB_FAULT_INJECTION -ErrorAction SilentlyContinue
    dotnet test './starter/RailYard.Access.Tests/RailYard.Access.Tests.csproj' -c Release --nologo
    if ($LASTEXITCODE -ne 0) { throw 'Normal test suite failed.' }

    dotnet run --project './starter/AcceptanceProbe/AcceptanceProbe.csproj' -c Release -- verify
    if ($LASTEXITCODE -ne 0) { throw 'Independent business contract failed.' }

    $env:LAB_FAULT_INJECTION = 'accept-expired'
    try {
        dotnet test './starter/RailYard.Access.Tests/RailYard.Access.Tests.csproj' -c Release --no-restore --nologo
        $mutationExit = $LASTEXITCODE
    }
    finally {
        Remove-Item Env:LAB_FAULT_INJECTION -ErrorAction SilentlyContinue
    }

    if ($mutationExit -eq 0) { throw 'Mutation gate failed: test suite remained green when expired passes were deliberately accepted.' }
    Write-Host 'VERIFY PASS: normal behavior correct and mutant is detected by tests.'
}
finally {
    Remove-Item Env:LAB_FAULT_INJECTION -ErrorAction SilentlyContinue
    Pop-Location
}
