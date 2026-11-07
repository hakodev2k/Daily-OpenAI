# Reference Solution — chỉ xem sau khi đã tự điều tra

## Symptoms

Hai dependency faulted nhưng report cuối cùng của starter chỉ chứa một lỗi.

## Evidence

`FAULTED_TASKS=2` trong khi `REPORTED_FAILURES=1`. Task đại diện cho `Task.WhenAll` chứa aggregate exception data cho nhiều operation faulted.

## Root cause

Starter coi exception được ném ra khi `await batchTask` như toàn bộ failure set. Với nhiều task cùng faulted, cách quan sát đó làm report chỉ giữ một exception thay vì đọc đầy đủ exception collection trên task tổng hợp.

## Why the fix works

Reference implementation giữ `batchTask`, chờ nó hoàn tất, rồi khi batch faulted đọc `batchTask.Exception.Flatten().InnerExceptions`. Như vậy report phản ánh mọi operation bị lỗi trong nhóm đã được chờ.

## How to verify

Sau khi sửa `starter/`:

```powershell
./verify.ps1
```

Kết quả cần có `FAULTED_TASKS=2`, `SUCCESSFUL_TASKS=1`, `REPORTED_FAILURES=2`, đồng thời có cả `TaxApi` và `BenefitsApi` trong report.

## Alternative fixes

Có thể giữ metadata theo từng task và sau `Task.WhenAll` kiểm tra từng task faulted để tạo structured failure report. Cách này hữu ích khi cần map lỗi về operation metadata rõ ràng.

## Wrong or misleading fixes

- Chạy tuần tự từng dependency: có thể khiến việc thu thập lỗi dễ hơn nhưng làm mất parallelism không cần thiết.
- Chỉ log exception trong `catch`: vẫn bỏ sót sibling failures.
- Retry toàn batch ngay lập tức: không sửa failure reporting và có thể tăng tải lên dependency đang lỗi.
- Swallow exception để job xanh: che giấu incident thay vì cải thiện observability.

## Production implications

Batch orchestration cần phân biệt fail-fast behavior, complete-all-and-report behavior và retry policy. Với independent I/O operations, report đầy đủ failure set giúp incident triage chính xác hơn.

## Trade-offs

Đọc aggregate exception collection phù hợp khi cần biết mọi lỗi sau khi toàn bộ operation đã hoàn tất. Nếu requirement cần dừng ngay khi lỗi đầu tiên xuất hiện, orchestration và cancellation policy sẽ khác.

## What a Senior engineer should notice

`async/await` quyết định cách control flow quan sát failure, còn task object vẫn mang state của asynchronous operation. Khi orchestrate nhiều task, cần thiết kế rõ contract về completion, cancellation, aggregation và reporting thay vì chỉ dựa vào exception đầu tiên được catch.
