$ErrorActionPreference = "Stop"
Push-Location "$PSScriptRoot/starter"
try { dotnet run -- --reproduce; if ($LASTEXITCODE -ne 0) { throw "Expected unsafe extraction symptom not reproduced" } } finally { Pop-Location }
