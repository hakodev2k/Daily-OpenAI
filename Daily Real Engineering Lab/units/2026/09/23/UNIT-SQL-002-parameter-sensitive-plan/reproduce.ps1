param([string]$Server='localhost')
$ErrorActionPreference='Stop'
Write-Host '=== Small tenant compiles first ==='
sqlcmd -S $Server -E -d EngineeringLabSql002 -Q "DBCC FREEPROCCACHE WITH NO_INFOMSGS; SET STATISTICS IO ON; SET STATISTICS TIME ON; EXEC dbo.GetTenantOrders 1,'2026-01-01'; EXEC dbo.GetTenantOrders 999,'2026-01-01';"
Write-Host '=== Large tenant compiles first ==='
sqlcmd -S $Server -E -d EngineeringLabSql002 -Q "DBCC FREEPROCCACHE WITH NO_INFOMSGS; SET STATISTICS IO ON; SET STATISTICS TIME ON; EXEC dbo.GetTenantOrders 999,'2026-01-01'; EXEC dbo.GetTenantOrders 1,'2026-01-01';"
Write-Host 'Compare logical reads, elapsed time, and the Actual Execution Plan in your SQL client.'