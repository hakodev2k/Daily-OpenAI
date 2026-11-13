$ErrorActionPreference='Stop'
Push-Location "$PSScriptRoot/starter"
try {
 dotnet build --nologo
 if($LASTEXITCODE -ne 0){throw 'Build failed'}
 $out = & dotnet run --no-build -- reproduce 2>&1 | Out-String
 if($out -notmatch 'ObjectDisposedException'){throw "Original symptom not reproduced: $out"}
 Write-Host "REPRODUCED: ObjectDisposedException"
 Write-Host $out
} finally {Pop-Location}
