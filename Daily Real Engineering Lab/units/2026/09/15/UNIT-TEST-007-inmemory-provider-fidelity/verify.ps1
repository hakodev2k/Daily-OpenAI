$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ProviderFidelity.Tests.csproj'
$source = Get-Content (Join-Path $PSScriptRoot 'starter/UserPersistenceTests.cs') -Raw
$csproj = Get-Content $project -Raw
if ($source -match 'UseInMemoryDatabase') { throw 'Verification failed: learner-editable test still uses the original non-relational provider.' }
if ($csproj -notmatch 'Microsoft.EntityFrameworkCore.Sqlite') { throw 'Verification failed: add a lightweight relational EF Core provider to the learner-editable test project.' }
dotnet restore $project
dotnet test $project --no-restore
if ($LASTEXITCODE -ne 0) { throw 'Verification failed: learner test suite is not green.' }
Write-Host 'Verified: learner test path uses relational semantics and the regression suite passes.'