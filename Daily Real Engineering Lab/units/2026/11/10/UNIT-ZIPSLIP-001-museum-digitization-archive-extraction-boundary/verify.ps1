$ErrorActionPreference = "Stop"
Push-Location "$PSScriptRoot/starter"
try { dotnet run -- --verify; if ($LASTEXITCODE -ne 0) { throw "Learner code failed safety/compatibility checks" } } finally { Pop-Location }
