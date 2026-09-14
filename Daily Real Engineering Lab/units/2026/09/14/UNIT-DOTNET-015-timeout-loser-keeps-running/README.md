# UNIT-DOTNET-015 — Request timeout nhưng work phía sau vẫn tiếp tục chạy

## Mục tiêu

Điều tra một async timeout boundary trả kết quả timeout đúng cho caller nhưng work phía sau vẫn tiếp tục chạy và giữ tài nguyên, khiến capacity giảm dần dưới load.

## Bối cảnh thực tế

Một Pricing Aggregation API gọi downstream provider có concurrency limit. Endpoint dùng timeout ngắn để phản hồi nhanh khi provider chậm. Functional response nhìn có vẻ đúng, nhưng sau một burst timeout, số operation đang chạy phía sau tiếp tục tăng và các request sau bắt đầu chậm theo.

## Bạn cần làm gì

- Reproduce symptom bằng starter.
- Ghi hypothesis trước khi sửa.
- Quan sát số downstream operation còn active sau khi caller đã nhận timeout.
- Sửa timeout boundary trong `starter/Program.cs`.
- Chạy `verify.ps1` để xác nhận timed-out work không còn sống ngoài lifetime mong muốn.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/14/UNIT-DOTNET-015-timeout-loser-keeps-running"
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Caller nhận timeout trong thời gian ngắn.
- Ngay sau nhiều timeout, downstream active-operation counter vẫn lớn hơn 0.
- Counter chỉ giảm sau khi simulated downstream work tự kết thúc.
- Timeout response không đồng nghĩa background operation đã dừng.

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

> Spoiler: chỉ xem sau khi đã tự điều tra.

[Reference Solution](solution/README.md)

## Expected Results

**Before**
- nhiều call trả timeout;
- active downstream work vẫn tồn tại sau timeout boundary.

**After**
- timed-out operation được yêu cầu dừng;
- timeout path quan sát completion/cancellation trước khi rời boundary;
- active-operation counter trở về 0 nhanh chóng.

## Estimated Time

30–45 phút.
