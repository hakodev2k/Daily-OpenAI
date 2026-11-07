$ErrorActionPreference="Stop"
dotnet run --project "$PSScriptRoot/../starter/ShelterFeed.csproj" -- verify
if($LASTEXITCODE -ne 0){throw "Learner starter failed regression"}
