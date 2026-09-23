USE EngineeringLabSql002;
GO
SET STATISTICS IO ON;
SET STATISTICS TIME ON;
GO
-- Learner-editable workload/fix area.
-- Preserve the procedure's result semantics while investigating plan behavior.
EXEC dbo.GetTenantOrders @TenantId=1,   @From='2026-01-01';
EXEC dbo.GetTenantOrders @TenantId=999, @From='2026-01-01';
GO