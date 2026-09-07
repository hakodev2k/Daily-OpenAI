$ErrorActionPreference = 'Stop'
$starter = Join-Path $PSScriptRoot 'starter'
Push-Location $starter
try {
    dotnet build --nologo
    $process = Start-Process dotnet -ArgumentList 'run --no-build --urls http://localhost:5088' -PassThru -WindowStyle Hidden
    try {
        Start-Sleep -Seconds 3

        $public = Invoke-WebRequest 'http://localhost:5088/public' -UseBasicParsing
        if ($public.StatusCode -ne 200) { throw "Expected /public = 200, got $($public.StatusCode)" }

        try {
            Invoke-WebRequest 'http://localhost:5088/secure' -UseBasicParsing | Out-Null
            throw 'Expected /secure without X-User to return 401.'
        }
        catch {
            if ($_.Exception.Response.StatusCode.value__ -ne 401) { throw }
        }

        $secure = Invoke-WebRequest 'http://localhost:5088/secure' -Headers @{ 'X-User' = 'ha' } -UseBasicParsing
        if ($secure.StatusCode -ne 200) { throw "Expected authenticated /secure = 200, got $($secure.StatusCode)" }
        if ($secure.Content -notmatch 'ha') { throw 'Expected response body to contain authenticated username.' }

        Write-Host 'PASS: learner-editable starter satisfies the required behavior.'
    }
    finally {
        if ($process -and -not $process.HasExited) { Stop-Process -Id $process.Id -Force }
    }
}
finally {
    Pop-Location
}
