$root=Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet run --project "$root/starter/ValueTaskLab.csproj"
if($LASTEXITCODE -ne 0){throw "Run failed"}
