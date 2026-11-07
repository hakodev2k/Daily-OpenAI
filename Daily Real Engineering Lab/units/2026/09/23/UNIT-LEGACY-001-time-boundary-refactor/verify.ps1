$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet run --project "$root/verifier/Verifier.csproj"
if ($LASTEXITCODE -ne 0) { throw "Verification failed. The learner-editable starter does not satisfy the replay/boundary contract." }