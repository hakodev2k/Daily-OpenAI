# Reference Solution — chỉ xem sau khi điều tra

## Symptoms
Telemetry báo upload bị từ chối và finalization cũng lỗi, nhưng incident chỉ ghi lỗi finalization.

## Evidence
Trong fixture `both`, `UploadCalls=1`, `DisposeCalls=1`, caller chỉ nhận `ArchiveFinalizeException`. `upload-only` và `finalize-only` cho thấy từng lỗi riêng vẫn hoạt động.

## Root cause
`await using` bảo đảm gọi `DisposeAsync` khi thoát scope. Nếu thân scope đã throw rồi `DisposeAsync` tiếp tục throw, ngoại lệ sau thay thế ngoại lệ trước trên đường unwind; caller mất nguồn gốc của lỗi upload. Đây là semantics của exception propagation, không phải việc retry hay timeout.

## Why the fix works
Reference [Program.cs](Program.cs) tách upload và cleanup thành hai pha, giữ `ExceptionDispatchInfo` riêng. Cleanup chạy đúng một lần kể cả khi upload lỗi. Nếu cả hai lỗi, trả về `AggregateException` có thứ tự rõ ràng. Nếu chỉ một lỗi, `ExceptionDispatchInfo.Throw()` giữ stack trace gốc. Không gọi lại upload để thu thập lỗi.

## How to verify
Sửa `starter/Program.cs` rồi chạy `./verify.ps1`; chỉ kiểm tra starter của học viên. Reference có thể tự kiểm tra bằng:
```powershell
dotnet run --project ./solution/VesselArchive.csproj -- verify
```
Kỳ vọng 4/4 PASS. `reproduce.ps1` chỉ xác nhận hành vi lỗi cũ.

## Alternative fixes and trade-offs
- Giữ primary exception và **log cleanup failure** có correlation ID: phù hợp khi public API bắt buộc giữ nguyên exception type, nhưng phải đảm bảo telemetry đáng tin cậy.
- Dùng `AggregateException`: phù hợp với worker diagnostics khi caller có hợp đồng hỗ trợ nhiều lỗi; cần cập nhật alert classification.
- Thực hiện cleanup riêng trong `finally` với try/catch và giữ exception ban đầu: hợp lý nếu policy ưu tiên nguyên nhân chính, nhưng đừng nuốt lỗi cleanup âm thầm.

## Wrong / tempting fixes
- `catch { }` quanh cleanup: che sự cố tài nguyên.
- `throw ex`: làm suy giảm stack provenance của lỗi đơn.
- Chỉ tăng retry hoặc scale worker: không giải quyết mất bằng chứng.
- Bỏ `DisposeAsync`: có nguy cơ rò rỉ tài nguyên.
- Gọi cleanup lần hai sau khi lỗi: có thể không idempotent và tạo tác dụng phụ.

## Production implications
Phải xác định error contract của worker, phân biệt lỗi upload với lỗi finalize, liên kết log bằng attempt ID, và theo dõi cả hai failure counters. Nếu cancellation là một trong hai lỗi, policy phải nêu rõ có giữ trạng thái cancelled hay bọc aggregate; reference chỉ minh họa hợp đồng của bốn fixture.

## What a Senior engineer should notice
Đường lỗi cũng là API contract. Thiết kế verification cho hai lỗi đồng thời, giữ stack provenance, không lặp side effects, và xác định rõ ưu tiên exception so với telemetry. Validation mode: static-review, chưa thực thi .NET.
