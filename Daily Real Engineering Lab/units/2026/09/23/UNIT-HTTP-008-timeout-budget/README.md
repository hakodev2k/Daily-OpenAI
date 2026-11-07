# UNIT-HTTP-008 — Downstream Timeout Budget

## Mục tiêu
Điều tra một API có tail latency cao khi dependency chậm, dùng evidence để xác định vì sao request đã thất bại ở phía caller nhưng backend vẫn tiếp tục tiêu thụ tài nguyên, sau đó sửa và verify behavior.

## Bối cảnh thực tế
Checkout API gọi Shipping Quote service. Bình thường request hoàn thành nhanh. Khi Shipping service chậm hoặc lỗi tạm thời, p95/p99 tăng mạnh, một số client nhận timeout, trong khi log backend vẫn xuất hiện activity của các request đó sau thời điểm client đã bỏ cuộc.

## Bạn cần làm gì
1. Chạy starter và reproduce incident.
2. Ghi lại timeline, số downstream attempts và tổng thời gian request.
3. Viết ít nhất 3 hypotheses trước khi sửa code.
4. Thay đổi starter để request tuân thủ latency contract và dừng work không còn hữu ích.
5. Chạy verify để kiểm tra functional behavior và failure-path behavior.

## Yêu cầu môi trường
- .NET SDK 8.0.x
- PowerShell 7+ hoặc Windows PowerShell
- Không cần dịch vụ cloud hay API bên ngoài.

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
- Thời điểm request bắt đầu và kết thúc.
- Số lần gọi dependency cho mỗi request.
- Request nào vượt quá latency contract.
- Có downstream activity nào tiếp tục sau khi caller-side deadline đã hết hay không.
- Cancellation có đi qua toàn bộ call path hay bị mất ở một boundary nào đó.

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
[Spoiler — chỉ mở sau khi đã thử fix](solution/README.md)

## Expected Results
Trước khi sửa, failure scenario có thể vượt latency contract và tạo work sau khi request không còn hữu ích. Sau khi sửa, request kết thúc trong budget hợp lý, cancellation dừng downstream work, và transient fast-recovery case vẫn có thể thành công.

## Estimated Time
45–60 phút.