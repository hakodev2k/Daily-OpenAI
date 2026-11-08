# UNIT-CHANNEL-001 — Cảnh báo lũ bị mất khi hàng đợi quá tải

## Mục tiêu
Điều tra một sự cố mất cảnh báo dưới burst traffic; thu thập bằng chứng và sửa đường đi dữ liệu mà không phá thứ tự xử lý.

## Bối cảnh thực tế
Trạm quan trắc sông phát 12 cảnh báo theo thứ tự. Trong thời gian gateway tạm dừng nhận, hệ thống vẫn báo publish thành công nhưng số thông báo nhận được thấp hơn.

## Bạn cần làm gì
Chạy starter, ghi lại số lượng và ID, đề xuất ít nhất hai hypothesis; sửa `starter/Program.cs` để giữ đầy đủ cảnh báo và xử lý producer/consumer không deadlock.

## Yêu cầu môi trường
Windows PowerShell 7 hoặc Windows PowerShell 5.1; .NET SDK 8.

## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Từ thư mục unit chạy `./reproduce.ps1`; ghi lại `Published`, `Delivered`, `IDs` và exit code.

## Những gì cần quan sát
Có exception không? Publish thành công có đồng nghĩa với delivery không? Các ID bị thiếu nằm ở vị trí nào? Khi gateway chậm, producer nên làm gì?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham khảo](solution/README.md).

## Expected Results
[Trước](expected-results/before.md) · [Sau](expected-results/after.md).

## Estimated Time
60 phút.
