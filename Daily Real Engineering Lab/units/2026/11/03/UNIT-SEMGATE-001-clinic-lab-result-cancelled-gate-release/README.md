# UNIT-SEMGATE-001 — Hủy yêu cầu nhưng hàng đợi xét nghiệm vẫn vượt giới hạn

## Mục tiêu
Điều tra lỗi giới hạn đồng thời trong dịch vụ chuyển kết quả xét nghiệm đến đối tác, sử dụng bằng chứng từ phép đo permit và cancellation.

## Bối cảnh thực tế
Gateway của phòng khám chỉ cho phép một tác vụ chuyển kết quả tại một thời điểm. Sau khi client hủy yêu cầu đang chờ, dashboard báo gateway có thêm năng lực xử lý dù không có tác vụ nào hoàn thành.

## Bạn cần làm gì
Reproduce sự cố, ghi giả thuyết, kiểm tra trạng thái trước/sau cancellation, sửa `starter/Program.cs` và chứng minh giới hạn đồng thời vẫn đúng khi có cancellation.

## Yêu cầu môi trường
.NET SDK 8 hoặc mới hơn, PowerShell trên Windows. Không cần dịch vụ ngoài hay NuGet package.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`; lệnh phải báo đã quan sát thấy sai lệch về số permit. Nếu không, lab fixture có vấn đề.

## Những gì cần quan sát
Ghi lại `CurrentCount` trước và sau yêu cầu bị hủy, và thử xem một tác vụ khác có thể vào gateway khi nó vẫn đang bận hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải và trade-offs](solution/README.md)

## Expected Results
[Trước sửa](expected-results/before.md) · [Sau sửa](expected-results/after.md)

## Estimated Time
45 phút.
