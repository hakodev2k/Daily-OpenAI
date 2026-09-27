$ErrorActionPreference="Stop"
dotnet run --project ./starter/ExceptionTimeline.csproj -- --reproduce
if($LASTEXITCODE -ne 0){throw "Expected timeline was not reproduced."}
Write-Host "REPRODUCED: policy observation occurs before cleanup."
