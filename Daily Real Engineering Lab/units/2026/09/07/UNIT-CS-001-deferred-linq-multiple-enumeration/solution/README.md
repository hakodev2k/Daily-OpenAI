# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## Symptoms

Business output đúng nhưng inventory dependency bị gọi 6 lần thay vì 3.

## Evidence

Cùng một `IEnumerable<Order>` bị consume bởi `Count()` và `foreach`. `CallCount` chứng minh predicate chạy lại.

## Root cause

LINQ trên `IEnumerable<T>` dùng deferred execution. `eligibleOrders` là pipeline chứ không phải snapshot dữ liệu. Mỗi enumeration thực thi lại `Where`.

## Why the fix works

`ToList()` materialize pipeline đúng một lần tại workflow boundary; những lần đọc sau dùng snapshot đã có.

## How to verify

```powershell
./verify.ps1
```

## Alternative fixes

- Single-pass processing.
- Tách I/O khỏi LINQ predicate.
- Với dataset lớn, streaming một lần thay vì materialize toàn bộ.

## Wrong / Tempting Fixes

- Cache ngầm trong dependency chỉ để che multiple enumeration.
- Đổi `Count()` thành `Any()`: giảm call nhưng không sửa execution model.
- Thêm parallelism: có thể tăng tải dependency.
- `ToList()` quá sớm ở data layer: tăng memory và có thể phá query composition.

## Production implications

Pattern này có thể thành duplicate SQL queries, duplicate HTTP calls, throttling và latency amplification.

## Trade-offs

`ToList()` đổi deferred streaming thành in-memory snapshot. Phù hợp khi collection nhỏ và cần reuse; dataset lớn có thể hợp với single-pass hơn.

## What a Senior engineer should notice

Materialization là một boundary có chủ đích; cần cân nhắc ownership, memory, consistency và consumption pattern.
