$ErrorActionPreference='Stop'
Push-Location "$PSScriptRoot/starter"
try { dotnet run } finally { Pop-Location }
