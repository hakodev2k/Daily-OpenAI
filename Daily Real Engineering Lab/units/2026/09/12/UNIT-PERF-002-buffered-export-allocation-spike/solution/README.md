# Reference Solution — UNIT-PERF-002

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## Symptoms

Functional output đúng, nhưng export path allocate thêm một vùng memory có kích thước xấp xỉ payload cho mỗi export. Khi nhiều export lớn chạy đồng thời, peak managed memory và GC pressure tăng nhanh dù business data không tăng.

## Evidence

Starter in ra `PAYLOAD_BYTES`, `ALLOCATED_BYTES` và `ALLOCATION_RATIO`. Với payload 8 MiB, allocation riêng trong `WriteExport` xấp xỉ một full payload vì intermediate buffer được cấp phát trước khi copy sang destination.

## Root cause

`WriteExport` tạo `MemoryStream` với capacity bằng toàn bộ payload, ghi payload vào đó, reset position rồi copy sang destination. Destination vốn đã là một stream có thể nhận bytes trực tiếp, nên intermediate full-payload buffer không mang lại giá trị cho contract hiện tại.

## Why the fix works

Ghi trực tiếp payload vào destination loại bỏ intermediate full-size allocation và một lượt copy. Data contract vẫn giữ nguyên: destination nhận chính xác cùng bytes theo cùng thứ tự.

```csharp
public static void WriteExport(Stream destination, ReadOnlySpan<byte> payload)
{
    destination.Write(payload);
}
```

## How to verify

Chạy:

```powershell
./verify.ps1
```

Verifier dùng chính `starter/` đã được learner sửa. Nó kiểm tra hai thuộc tính độc lập:

1. bytes output vẫn giống payload gốc;
2. allocation trong export path nhỏ hơn 1 MiB cho payload 8 MiB.

## Alternative fixes

- Nếu producer sinh dữ liệu theo từng chunk, stream từng chunk trực tiếp tới destination thay vì tạo full payload trước.
- Nếu API downstream bắt buộc cần seekable full buffer, buffering có thể hợp lệ; lúc đó cần giới hạn concurrency, kích thước payload và đo peak memory thay vì xóa buffer một cách máy móc.
- Với pipeline async thật, dùng `WriteAsync`/`CopyToAsync` và propagation `CancellationToken` khi I/O có thể block.

## Wrong or misleading fixes

- Tăng memory limit của container: chỉ kéo dài thời gian trước khi pressure xuất hiện.
- Gọi `GC.Collect()` sau mỗi export: tăng pause/CPU và không sửa allocation pattern.
- Chuyển buffer sang `byte[]` khác: vẫn giữ full-size intermediate copy.
- Scale out ngay: có thể tăng capacity tạm thời nhưng không loại bỏ amplification trên từng request/job.

## Production implications

Một allocation 8 MiB có thể không đáng kể ở một request, nhưng 20–50 export đồng thời biến nó thành hàng trăm MiB transient memory. Large allocations còn làm GC behavior và peak RSS phức tạp hơn, đặc biệt khi payload thực tế lớn hơn lab.

## Trade-offs

Streaming trực tiếp giảm memory và copy cost nhưng làm rollback/retry ở giữa stream khó hơn nếu destination không hỗ trợ transaction semantics. Buffering vẫn có thể đúng khi cần compute checksum trước khi gửi, random access, retry toàn bộ payload hoặc atomic publish. Quyết định phải dựa trên contract thật.

## What a Senior engineer should notice

- Correctness test không đo resource behavior.
- Peak memory là hàm của allocation per operation × concurrency, không chỉ kích thước payload đơn lẻ.
- Cần phân biệt buffering có chủ đích vì contract với buffering vô tình vì implementation convenience.
- Tối ưu tốt nhất thường là xóa copy không cần thiết trước khi thêm pooling, tuning GC hoặc scale-out.
