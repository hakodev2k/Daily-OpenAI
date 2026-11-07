# After
Verify trên starter đã sửa trả exit code 0: SHIP-1001 và SHIP-1002 trả 202 / accepted:<eventId>; malformed JSON và body rỗng vẫn 400; auditBytes của shipment hợp lệ giữ nguyên. Reproduce có thể trả 1 sau khi fix vì lỗi ban đầu đã biến mất. Tự kiểm tra reference: dotnet run --project solution/AuditLab.Solution.csproj -- verify.
