$ErrorActionPreference='Stop'
dotnet run --project "$PSScriptRoot/starter/MuseumRegistry.csproj"
if($LASTEXITCODE -ne 0){exit $LASTEXITCODE}
