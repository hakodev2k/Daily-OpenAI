# Reference Solution — chỉ xem sau khi đã tự điều tra

## 1. Symptoms

Source chuyển từ `v1` sang `v2`, nhưng hot tenant tiếp tục nhận `v1`. Cold tenant lại refresh bình thường.

## 2. Evidence

Starter cập nhật `LastAccessUtc` ở mỗi cache hit. Với access cách nhau 4 phút và sliding window 5 phút, entry luôn được xem là còn hợp lệ. `CreatedUtc` tồn tại nhưng không tham gia expiration decision.

## 3. Root cause

Cache policy chỉ dùng sliding expiration. Read traffic liên tục reset idle clock, nên một hot entry có thể sống vô hạn và vượt quá freshness requirement của business.

## 4. Why the fix works

Reference solution giữ sliding expiration để loại bỏ entry ít dùng nhưng bổ sung một absolute maximum age kể từ `CreatedUtc`. Khi total age vượt giới hạn, request tiếp theo buộc reload từ source dù key vừa được đọc gần đây.

## 5. How to verify

```powershell
./verify.ps1
```

Acceptance criteria:

- tại phút 12, hot tenant phải nhận `v2`
- source không bị gọi ở mọi request
- phút 13 vẫn nhận `v2` từ cache vừa refresh

## 6. Alternative fixes

- Chỉ dùng absolute expiration nếu sliding behavior không mang giá trị đáng kể.
- Versioned cache key hoặc explicit invalidation khi configuration publish event đáng tin cậy.
- Short TTL đơn giản nếu source load thấp và business chấp nhận refresh thường xuyên.

## 7. Wrong / tempting fixes

- Tăng sliding TTL: chỉ kéo dài stale window và không tạo freshness bound.
- Restart service sau mỗi config change: che symptom bằng operational workaround.
- Disable cache hoàn toàn: đảm bảo freshness nhưng có thể phá latency/capacity; chỉ hợp lý nếu measurement cho thấy cache không cần thiết.
- Thêm background refresh mà không định nghĩa stale policy: tăng complexity nhưng vẫn có thể phục vụ stale data khi refresher lỗi.

## 8. Production implications

Với Redis hoặc distributed cache, cần phân biệt rõ TTL semantics, invalidation ownership, clock/failure behavior và cache-aside race. Không nên suy luận freshness SLA chỉ từ một sliding TTL.

## 9. Trade-offs

Absolute max age nhỏ hơn cải thiện freshness nhưng tăng source load. Sliding window giúp giữ hot data nhưng không thể là freshness guarantee. Event-driven invalidation giảm stale time nhưng tăng coupling và yêu cầu xử lý missed/duplicate events.

## 10. What a Senior engineer should notice

Cache policy phải bắt đầu từ consistency/freshness requirement, không phải từ một TTL tùy ý. Senior engineer cần hỏi: dữ liệu được phép stale bao lâu, điều gì invalidate nó, source chịu được miss rate nào, và hệ thống xử lý cache/source failure ra sao.
