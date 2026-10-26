$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    dotnet run --project ./starter/RangeLab/RangeLab.csproj -- --expect-fixed
    if ($LASTEXITCODE -ne 0) { throw "VERIFY FAILED: one or more learner-workspace fixtures remain incorrect." }
    Write-Host "VERIFIED: all learner-workspace fixtures match their original byte streams."
} finally { Pop-Location }
