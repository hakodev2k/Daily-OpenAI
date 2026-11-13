$ErrorActionPreference='Stop'
Push-Location "$PSScriptRoot/starter"
try {
 dotnet build --nologo
 if($LASTEXITCODE -ne 0){throw 'Build failed'}
 & dotnet run --no-build -- verify
 if($LASTEXITCODE -ne 0){throw 'Learner code failed verification'}
} finally {Pop-Location}
