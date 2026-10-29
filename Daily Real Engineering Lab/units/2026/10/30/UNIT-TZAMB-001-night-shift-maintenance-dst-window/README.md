# UNIT-TZAMB-001 — Ca bảo trì đêm bị lệch giờ khi đồng bộ lịch quốc tế

## Mục tiêu

Điều tra một sai lệch lịch bảo trì tại ranh giới chuyển giờ địa phương; cải tiến hợp đồng chuyển đổi thời gian mà không làm hỏng các lịch hợp lệ. Luyện kỹ năng refactoring có kiểm chứng bằng regression contract.

## Bối cảnh thực tế

Nền tảng điều phối bảo trì nhận lịch từ đối tác tại New York, lưu thời điểm thực hiện dưới dạng UTC để phân công nhân sự. Một số yêu cầu trong đêm đổi giờ bị lệch đúng 60 phút; các yêu cầu ban ngày vẫn hoạt động. Đối tác cho rằng dữ liệu gửi đi chính xác.

## Bạn cần làm gì

1. Chạy starter và ghi lại các trường hợp sai khác.
2. Phân loại dữ liệu đầu vào, bằng chứng và ít nhất hai giả thuyết.
3. Đề xuất hợp đồng xử lý dữ liệu không đủ thông tin, dữ liệu không hợp lệ và dữ liệu hợp lệ.
4. Refactor `starter/WindowScheduler.cs` để toàn bộ regression contract đạt.
5. Giải thích hệ quả khi triển khai cho các đối tác đang dùng hợp đồng cũ.

## Yêu cầu môi trường

- .NET SDK 8.0 hoặc mới hơn có khả năng build `net8.0`; cần .NET 8 runtime để chạy ứng dụng.
- PowerShell trên Windows; không cần database, Docker, cloud hoặc NuGet ngoài SDK.

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/10/30/UNIT-TZAMB-001-night-shift-maintenance-dst-window"
./setup.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề

`./reproduce.ps1` chạy các contract cases trên starter nguyên bản. Kết quả thành công của bước reproduce là **tái hiện chính xác ba case không đạt** và script trả exit code 0. Nếu bạn đã sửa starter, script này có thể không còn đạt kỳ vọng của baseline; dùng `./verify.ps1` để kiểm tra bản sửa.

## Những gì cần quan sát

Đối chiếu input của từng case với UTC được hệ thống tính ra, các trường hợp bị từ chối và thời điểm nghiệp vụ mong đợi. Xem thêm `evidence/incident-log.md` và `docs/scenario.md`. Chú ý các case thành công để tránh sửa quá rộng.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler — chỉ xem sau khi tự thử:** [Giải pháp tham khảo](solution/README.md).

## Expected Results

- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)

`./verify.ps1` kiểm tra **chính file starter mà bạn chỉnh sửa**, không chạy reference solution.

## Estimated Time

70 phút. Ghi kết quả vào [investigation workspace](workspace/my-investigation.md).
