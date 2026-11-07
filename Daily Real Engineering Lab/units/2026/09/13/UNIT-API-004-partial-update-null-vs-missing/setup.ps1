$ErrorActionPreference = 'Stop'
$program = Get-Content "$PSScriptRoot/starter/Program.cs" -Raw
dotnet new console --framework net8.0 --force --no-restore --output "$PSScriptRoot/starter" | Out-Null
Set-Content "$PSScriptRoot/starter/Program.cs" $program -NoNewline
