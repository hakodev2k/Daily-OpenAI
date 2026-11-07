# UNIT-GETORADD-001 — Hai lần cấp ngăn tủ cho một kiện hàng

## Mục tiêu
Phân tích một incident concurrency khi cache hiển thị một kết quả nhưng hệ thống đối tác ghi nhận nhiều side effect; thu thập bằng chứng và thiết kế regression test có kiểm soát.

## Bối cảnh thực tế
Dịch vụ điều phối tủ nhận hàng cho phép nhiều thiết bị đồng thời yêu cầu vị trí của cùng một kiện. Trong một đợt tải cao, báo cáo đối tác có hai lần cấp chỗ cho cùng mã kiện, trong khi bộ nhớ ứng dụng chỉ lưu một reservation.

## Bạn cần làm gì
Chạy harness, ghi nhận số lần cấp chỗ và số mục trong cache, đề xuất ít nhất hai giả thuyết; sửa `starter/Program.cs` sao cho mỗi mã kiện chỉ có một side effect trong cùng đợt yêu cầu. Kiểm tra thêm khi thao tác thất bại.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7+. Không cần cloud, database hoặc NuGet package ngoài.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script chạy hai yêu cầu đồng thời và kiểm tra sự khác biệt giữa trạng thái ghi nhận của gateway và cache. Exit code 0 của reproduce nghĩa là triệu chứng được tái hiện.

## Những gì cần quan sát
Ghi số lần gọi gateway, số entry cache, kết quả trả về cho hai caller và hành vi sau một lần cấp chỗ thất bại.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong starter.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải và trade-offs](solution/README.md).

## Expected Results
[Trước](expected-results/before.md) và [sau](expected-results/after.md).

## Estimated Time
45–60 phút.
