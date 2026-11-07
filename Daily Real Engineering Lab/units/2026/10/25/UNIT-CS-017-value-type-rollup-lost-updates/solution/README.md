# Reference Solution — chỉ mở sau khi đã tự reproduce và thử sửa

## 1. Symptoms
Batch nhận 4 sự kiện, log xác nhận input nhưng dashboard cho cả hai kỹ thuật viên tổng 0.

## 2. Evidence
`Ingest()` gọi `Add()` đủ 4 lần. `Read()` dùng cùng key như khi ghi, nhưng dictionary không có giá trị cập nhật. Không có exception và không có thao tác bất đồng bộ.

## 3. Root cause
`WorkSummary` là mutable `struct`, tức value type. `Dictionary<TKey,TValue>.TryGetValue` trả giá trị ra biến `out` bằng cách **copy**. `current.Record()` chỉ thay đổi bản sao cục bộ. Starter không gán giá trị đã cập nhật trở lại dictionary; ngay cả key mới cũng không được thêm.

## 4. Why the fix works
Reference giữ nguyên value-type contract, gọi `TryGetValue`, cập nhật biến cục bộ rồi thực hiện `_byTechnician[entry.TechnicianId] = current`. Ghi lại sau mỗi sự kiện làm cho state tồn tại qua các lần `Add()`; `Read()` vẫn trả snapshot độc lập.

Xem [reference code](DailyRollup.cs). Chỉ đối chiếu sau khi tự sửa `starter/DailyRollup.cs`.

## 5. Verify
```powershell
./verify.ps1
dotnet run --project solution/FieldOpsLab.Reference.csproj -- verify
```
Lệnh thứ nhất kiểm tra **starter người học đã sửa**; lệnh thứ hai kiểm tra reference độc lập. Kỳ vọng cả hai exit 0; `reproduce.ps1` sau fix sẽ không còn chứng minh lỗi cũ.

## 6. Alternative fixes
- Dùng immutable `record struct` và `with` để tạo giá trị mới rồi gán lại dictionary; thích hợp nếu muốn tránh mutable struct.
- Dùng reference type `class` cho accumulator: các phương thức có thể thay đổi object đã lưu, nhưng `Read()` phải trả DTO/snapshot để tránh rò rỉ mutable state. Không thay kiểu tùy tiện khi contract đã yêu cầu snapshot.
- Với tổng hợp có cạnh tranh nhiều thread, cần thiết kế thêm synchronization hoặc atomic aggregation; reference chỉ giải quyết **mất cập nhật do copy**, không hứa thread safety.

## 7. Wrong / tempting fixes
- Gọi `Record()` nhiều lần hơn: chỉ thay đổi bản sao nhiều lần.
- Thêm `lock` quanh starter: tuần tự hóa một thao tác vốn không lưu lại state; không khắc phục nguyên nhân.
- Chuyển toàn bộ dictionary sang `ConcurrentDictionary` nhưng giữ logic read-copy-update không có write-back: vẫn không đảm bảo kết quả; kể cả có write-back, race condition cần xử lý riêng.
- Chỉnh `Program.cs` để hardcode tổng: che giấu sai lệch giữa nguồn sự kiện và aggregate.

## 8. Production implications
Nếu dữ liệu được dùng để đánh giá SLA, dashboard 0 sai có thể gây báo cáo và thanh toán sai. Telemetry nên phân biệt accepted events, updated aggregates và invalid keys; cần kiểm tra tính nhất quán khi ingest và khi xuất report.

## 9. Trade-offs
Mutable struct tránh một số cấp phát nhưng dễ gây nhầm lẫn về copy semantics. Immutable value object làm thay đổi state rõ ràng hơn. Reference tối thiểu giúp học cơ chế; ở production có thể cân nhắc kiểu dữ liệu immutable hoặc aggregate API có ownership rõ ràng.

## 10. Senior engineer should notice
Phải tách **data-loss bug** khỏi concurrency bug. Một bài test đơn luồng tái hiện lỗi trước khi bàn đến lock hoặc scale. Kiểm tra cả key chưa tồn tại, nhiều update trên cùng key, cách ly key và snapshot contract. Sau đó mới đánh giá thread safety và persistence nếu hệ thống thực tế cần.
