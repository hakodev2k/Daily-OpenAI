> Reference Solution — chỉ xem sau khi đã reproduce và thử fix.

# Reference Solution

## Symptoms

Worker B takeover partition chỉ vài giây sau khi worker A bắt đầu, khiến hai worker cùng xử lý một batch.

## Evidence

Timeline cho thấy storage time mới tiến 5 giây nhưng worker B gửi timestamp đã tiến 45 giây. CPU, memory, queue depth và network đều bình thường nên overload không giải thích được premature takeover.

## Root cause

Lease store dùng `workerObservedNow` do caller cung cấp để quyết định lease đã hết hạn hay chưa. Khi clock của worker B lệch nhanh 40 giây, nó làm store tin rằng lease của worker A đã hết hạn dù storage authority mới chỉ tiến 5 giây.

## Why the fix works

Quyết định expiry phải dùng một time authority nhất quán ở nơi sở hữu lease state. Reference solution dùng `_storageClock.UtcNow` cho cả so sánh expiry và tạo `ExpiresAtUtc`, nên clock skew giữa các worker không thể tự tạo premature takeover.

## How to verify

Chạy:

```powershell
./verify.ps1
```

Verifier kiểm tra hai property độc lập:

- worker B không takeover ở +5 giây dù local clock của nó nhanh 40 giây;
- worker B vẫn takeover thành công sau khi storage time vượt TTL.

## Alternative fixes

- Trong database thật, thực hiện acquisition bằng server-side time như `SYSUTCDATETIME()`/database equivalent trong cùng atomic statement hoặc transaction phù hợp.
- Dùng distributed coordination service có lease/lock semantics rõ ràng thay vì tự xây protocol nếu operational constraints cho phép.
- Với critical side effects, bổ sung fencing token tăng đơn điệu để downstream từ chối stale owner ngay cả khi lease ownership thay đổi trong lúc work vẫn đang chạy.

## Wrong or misleading fixes

- Tăng TTL chỉ làm giảm xác suất, không sửa trust model của clock.
- Đồng bộ NTP tốt hơn là cần thiết cho operations nhưng không nên biến correctness của lease thành giả định rằng mọi node luôn có clock giống nhau tuyệt đối.
- Retry acquisition nhiều lần không sửa premature expiry decision.
- Chỉ kiểm tra owner trước khi bắt đầu work vẫn không bảo vệ side effect dài nếu ownership thay đổi trong lúc xử lý.

## Production implications

Lease protocol là distributed correctness boundary. Timestamp do worker cung cấp nên được xem là observation, không phải authority, trừ khi protocol đã chứng minh được trust model đó.

## Trade-offs

Storage-authoritative time tạo dependency vào storage semantics và latency nhưng cung cấp một ordering/time reference nhất quán hơn. Fencing token mạnh hơn cho side effects nhưng yêu cầu downstream lưu/so sánh token.

## What a Senior engineer should notice

Một row lease duy nhất không tự động bảo đảm single-owner execution. Cần phân biệt state ownership, time authority và protection của downstream side effects. Khi correctness quan trọng, hãy xác định rõ authority nào quyết định expiry và stale worker bị chặn ở đâu.
