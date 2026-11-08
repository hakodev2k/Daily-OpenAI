$p = Join-Path $PSScriptRoot "../starter/IdentityLab.csproj"
dotnet run --project $p -- reproduce
exit $LASTEXITCODE
