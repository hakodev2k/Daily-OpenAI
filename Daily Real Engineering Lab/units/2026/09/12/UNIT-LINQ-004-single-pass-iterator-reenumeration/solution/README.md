# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## Symptoms

Batch tổng hợp ra đúng `Total`, nhưng query kế tiếp trên cùng biến `payments` trả về không có giao dịch high-risk.

## Evidence

`read:*` chỉ xuất hiện trong lần enumeration đầu. Sau phép `Sum`, nguồn `Queue<Payment>` đã không còn phần tử để lần enumeration tiếp theo đọc.

## Root cause

`ReadPayments` là iterator dùng `yield return`, vì vậy `payments` chỉ là một deferred `IEnumerable<Payment>`, không phải snapshot. Iterator còn đọc từ một nguồn có tính consuming: mỗi lần `TryDequeue` làm thay đổi `Queue`. `Sum` enumeration lần đầu làm cạn nguồn; `Where` enumeration lần hai vì thế nhìn thấy tập rỗng.

## Why the fix works

Materialize đúng một lần tại ownership boundary bằng `ToList()`. Từ thời điểm đó các phép tính nghiệp vụ đọc cùng một snapshot ổn định. Với batch nhỏ như lab, chi phí O(n) memory là hợp lý để đổi lấy semantics rõ ràng và nhiều phép đọc an toàn.

## How to verify

Chạy `verify.ps1`. Script kiểm tra hai input độc lập và xác nhận cả tổng tiền lẫn số giao dịch high-risk từ chính `starter/` mà bạn sửa.

## Alternative fixes

- Với stream rất lớn, xử lý một pass và cập nhật nhiều aggregate cùng lúc để tránh materialize toàn bộ.
- Thay contract của ingestion adapter để trả về một snapshot/read-only collection nếu caller cần re-enumeration.
- Chỉ re-read nguồn khi nguồn đó thực sự replayable và semantics nghiệp vụ cho phép một lần đọc mới.

## Wrong / tempting fixes

- Đổi thứ tự `Sum` và `Where`: chỉ chuyển kết quả sai sang phép tính còn lại.
- Gọi `ReadPayments(source)` hai lần: cả hai vẫn chia sẻ cùng `Queue` đang bị tiêu thụ.
- Gọi `ToList()` sau lần enumeration đầu: dữ liệu đã mất khỏi nguồn.
- Materialize mọi `IEnumerable<T>` theo thói quen: có thể gây allocation lớn và phá lợi ích streaming khi source vốn replayable.

## Production implications

`IEnumerable<T>` chỉ mô tả khả năng enumeration; nó không đảm bảo source replayable, immutable hay idempotent qua nhiều lần enumeration. Database readers, network streams, message adapters và custom iterators có thể mang semantics single-pass hoặc stateful tương tự.

## Trade-offs

Materialization giúp consistency và đơn giản hóa nhiều phép đọc nhưng tốn O(n) memory. One-pass aggregation tiết kiệm memory hơn nhưng gắn các phép tính vào cùng pipeline và có thể làm code khó mở rộng.

## What a Senior engineer should notice

Contract nên thể hiện rõ ownership và replay semantics. Nếu caller cần nhiều lần đọc, đừng để một `IEnumerable<T>` mơ hồ che giấu một nguồn consuming.

## References

- https://learn.microsoft.com/en-us/dotnet/csharp/linq/get-started/introduction-to-linq-queries
- https://learn.microsoft.com/en-us/dotnet/standard/linq/deferred-execution-lazy-evaluation
- https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1.getenumerator
