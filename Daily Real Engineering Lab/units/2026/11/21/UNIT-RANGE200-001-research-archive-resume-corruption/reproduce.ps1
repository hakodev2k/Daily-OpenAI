$ErrorActionPreference='Stop'
Push-Location $PSScriptRoot
try {
 $out=& dotnet run --project ./starter/ArchiveResume.csproj 2>&1 | Out-String
 if($out -notmatch 'INTEGRITY_MISMATCH' -or $out -notmatch 'status=200'){throw "Cannot reproduce: $out"}
 Write-Host "REPRODUCED: $out"
} finally {Pop-Location}
