$ErrorActionPreference='Stop'
Push-Location $PSScriptRoot
try {
 $out=& dotnet run --project ./starter/ArchiveResume.csproj 2>&1 | Out-String
 if($LASTEXITCODE -ne 0 -or $out -notmatch 'INTEGRITY_OK' -or $out -match 'INTEGRITY_MISMATCH'){throw "Learner starter failed: $out"}
 Write-Host "VERIFIED: $out"
} finally {Pop-Location}
