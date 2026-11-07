# UNIT-FENCE-001 — Bản kế hoạch tuyến xe bị ghi đè sau khi worker chuyển quyền

## Mục tiêu
Điều tra một lỗi nhất quán dữ liệu khi hai worker lần lượt được phép tính lại kế hoạch tuyến xe. Thu thập timeline, đề xuất giả thuyết, sửa đường ghi kết quả và chứng minh bản kế hoạch mới không bị thay thế bởi công việc hoàn tất muộn.

## Bối cảnh thực tế
Dịch vụ lập tuyến xe chạy theo từng khu vực. Một worker xử lý lâu hơn dự kiến; worker khác được giao tiếp công việc và lưu kế hoạch mới. Vài giây sau, bảng điều phối hiển thị lại kế hoạch cũ, dù cả hai worker đều ghi log “đã lưu thành công”. Một đợt scale-out khiến lỗi xuất hiện thường xuyên hơn.

Lab dùng mô phỏng in-memory có đồng hồ điều khiển bằng tay. Không cần Redis, Azure, SQL Server, Docker hoặc subscription trả phí.

## Bạn cần làm gì
1. Chạy mô phỏng và ghi lại thứ tự cấp quyền, hết hạn, commit và kết quả cuối.
2. Đưa ra ít nhất hai hypothesis; xác định bằng chứng phân biệt lỗi scheduling, clock và storage.
3. Chỉ chỉnh sửa `starter/LeaseAndPlan.cs`; không thay đổi `harness/`.
4. Chạy `verify.ps1`, bảo đảm cả đường lỗi và các trường hợp bình thường đều đúng.
5. So sánh với reference solution và nêu giới hạn của mô phỏng khi áp dụng vào storage phân tán.

## Yêu cầu môi trường
.NET 8 SDK (8.0.100 trở lên trong major 8) và PowerShell 7 hoặc Windows PowerShell 5.1. Không dùng NuGet package ngoài framework.

## Chạy nhanh
Từ thư mục unit:
```powershell
.\scripts\setup.ps1
.\scripts\run.ps1
.\scripts\reproduce.ps1
```

## Cách reproduce vấn đề
```powershell
.\scripts\reproduce.ps1
```
Ở trạng thái starter, exit code 0 cùng `EXPECTED_INCIDENT_REPRODUCED` nghĩa là đã tái hiện lỗi, **không** có nghĩa hệ thống đúng.

## Những gì cần quan sát
- Thứ tự và thời điểm hai worker nhận quyền xử lý cùng một tuyến.
- Worker nào ghi kết quả trước; worker nào hoàn thành sau.
- Giá trị cuối trong kho kế hoạch và thông tin đi kèm bản ghi.
- Một phép kiểm thử chỉ kiểm tra “cả hai lệnh đều thành công” có đủ bảo vệ dữ liệu không?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong [workspace](workspace/my-investigation.md).
3. Thử fix trong `starter/LeaseAndPlan.cs`.
4. Verify bằng `.\scripts\verify.ps1` (phải exit 0).
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ đọc sau khi tự điều tra:** [Phân tích và code tham chiếu](solution/README.md).

## Expected Results
[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md)

## Estimated Time
50–65 phút (L2 Investigation, D4).
