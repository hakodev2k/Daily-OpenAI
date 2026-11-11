# UNIT-CONFIGSNAP-001 — Library Kiosk: Printer Settings After Refresh

## Mục tiêu
Refactor một thành phần .NET chạy lâu để đáp ứng thay đổi cấu hình runtime mà không phá vỡ tính nhất quán của mỗi lần in.

## Bối cảnh thực tế
Quầy mượn sách tại thư viện chuyển máy in nhãn giữa ca trực. Trang quản trị xác nhận cấu hình mới, nhưng một số nhãn tiếp theo vẫn được phát hành theo cấu hình trước đó.

## Bạn cần làm gì
1. Chạy baseline và lưu output.
2. Đưa ra hai giả thuyết: đường cập nhật cấu hình và đường đọc cấu hình.
3. Sửa `starter/Program.cs` để nhãn sau thay đổi sử dụng cấu hình mới, trong khi nhãn đã in không bị đổi.
4. Chạy verification và đánh giá trade-off khi cấu hình thay đổi đồng thời với nhiều request.

## Yêu cầu môi trường
.NET SDK 8; PowerShell 7+; không cần Docker hoặc dịch vụ ngoài.

## Chạy nhanh
```powershell
pwsh ./scripts/run.ps1
pwsh ./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `pwsh ./scripts/reproduce.ps1` từ thư mục unit, trước khi sửa starter. Script báo `REPRODUCED` khi thấy sự khác biệt giữa cấu hình đang hoạt động và nhãn in.

## Những gì cần quan sát
Ghi lại nhãn trước và sau thao tác quản trị, version cấu hình, và giá trị cấu hình được hệ thống công bố. Đừng kết luận nguyên nhân chỉ từ một dòng log.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham khảo](solution/README.md).

## Expected Results
Nhãn đầu giữ giá trị ban đầu; nhãn thứ hai phản ánh cấu hình mới. [Bảng kết quả](expected-results/results.md).

## Estimated Time
45 phút (L4 / D4).

