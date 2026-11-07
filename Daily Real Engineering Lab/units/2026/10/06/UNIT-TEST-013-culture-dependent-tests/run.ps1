$root=Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet run --project "$root/starter/CultureLab.csproj" -- en-US
