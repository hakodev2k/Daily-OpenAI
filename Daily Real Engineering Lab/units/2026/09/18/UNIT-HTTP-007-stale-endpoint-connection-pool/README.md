# UNIT-HTTP-007 — Stale Downstream Endpoint After DNS Rotation

## Mục tiêu
Điều tra vì sao một service .NET tiếp tục gửi request tới endpoint cũ sau khi downstream đã chuyển sang endpoint mới.

## Bối cảnh thực tế
Pricing API gọi một partner service qua hostname ổn định. Trong một deployment, DNS record được đổi sang instance mới. Một số process chuyển sang endpoint mới nhanh, nhưng một process vẫn tiếp tục nhận response từ instance cũ trong thời gian dài. Restart process làm hiện tượng biến mất.

## Bạn cần làm gì
1. Chạy starter và reproduce hiện tượng.
2. Ghi lại endpoint mà từng request thực sự sử dụng trước và sau DNS rotation.
3. Đưa ra ít nhất hai hypothesis.
4. Sửa learner-editable code trong `starter/`.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell
- Không cần DNS server hay partner service thật; lab dùng deterministic simulator.

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
- Hostname logic không đổi trong suốt scenario.
- Endpoint thực tế được dùng cho từng request.
- Thời điểm simulated DNS record thay đổi.
- Sự khác biệt giữa state của resolver và state mà request path đang sử dụng.

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
[Reference Solution — spoiler](solution/README.md)

## Expected Results
Before: sau DNS rotation, ít nhất một request vẫn đi tới endpoint cũ.

After: request path chuyển sang endpoint mới trong giới hạn lifecycle đã định mà không cần restart process.

Nếu không reproduce được, chạy lại từ repository sạch và bảo đảm chưa sửa `starter/`.

## Estimated Time
50 phút.