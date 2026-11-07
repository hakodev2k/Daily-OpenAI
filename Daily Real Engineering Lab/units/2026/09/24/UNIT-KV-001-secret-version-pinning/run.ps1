$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet run --project "$root/starter/SecretRotationLab.csproj"
if ($LASTEXITCODE -ne 0) { throw "Starter run failed." }