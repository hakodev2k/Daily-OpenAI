# UNIT-REDIS-002 — Distributed lock mất quyền sở hữu giữa chừng

## Mục tiêu

Điều tra một concurrency incident trong đó nhiều worker tin rằng chúng đang bảo vệ cùng một critical section bằng Redis-style distributed lock, nhưng trong một timing window hiếm vẫn có hai worker cùng thực thi.

## Bối cảnh thực tế

Một background worker xử lý cùng một `customerId` trên nhiều instance. Hệ thống dùng lock có TTL để tránh deadlock khi process crash. Bình thường mọi thứ chạy đúng, nhưng khi một operation kéo dài vượt TTL rồi worker khác lấy được lock, log đôi lúc cho thấy critical section bị overlap.

Starter dùng một `FakeRedisLockStore` deterministic để mô phỏng semantics `SET NX PX` và release mà không cần cài Redis.

## Bạn cần làm gì

- Chạy `reproduce.ps1` và quan sát ownership timeline.
- Ghi hypothesis về thời điểm quyền sở hữu lock thay đổi.
- Sửa starter để một worker chỉ có thể release lock mà chính nó vẫn còn sở hữu.
- Chạy `verify.ps1` để xác nhận không còn overlap trong scenario đã cho.
- Sau đó mới so sánh với reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7 hoặc Windows PowerShell

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/14/UNIT-REDIS-002-expired-lock-owner-release"
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Scenario deterministic:

1. Worker A lấy lock.
2. Thời gian logic tiến qua TTL của A.
3. Worker B lấy lock mới trên cùng key.
4. Worker A hoàn tất operation cũ và release.
5. Worker C thử lấy lock khi B vẫn đang ở critical section.

## Những gì cần quan sát

- A và B đều từng acquire thành công, nhưng ở hai thời điểm khác nhau.
- B vẫn chưa kết thúc critical section khi A thực hiện cleanup của operation cũ.
- Sau cleanup đó, C có thể acquire trong starter ban đầu.
- Hãy tập trung vào **ownership evidence**, không chỉ vào việc key đang tồn tại hay không.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ xem sau khi đã thử fix.

[Reference Solution](solution/README.md)

## Expected Results

**Before**

- Scenario kết thúc với `VIOLATION` vì C acquire trong khi B vẫn đang giữ critical section theo business timeline.

**After**

- Cleanup của A không phá ownership hiện tại của B.
- C không acquire được cho tới khi B release đúng ownership của mình.
- `verify.ps1` kết thúc thành công.

## Estimated Time

35–50 phút.
