# UNIT-SEMCANCEL-001 — Greenhouse irrigation gate: cancellation và quyền sở hữu permit

## Mục tiêu
Điều tra một lỗi điều phối bất đồng bộ khi tác vụ tưới bị hủy trong lúc bộ điều khiển đang bận; xác minh mutual exclusion và cleanup đúng khi có cancellation.

## Bối cảnh thực tế
Hệ thống nhà kính chỉ cho một lệnh điều khiển van hoạt động tại một thời điểm. Khi một lệnh đang chạy, lệnh tiếp theo có thể bị người vận hành hủy. Dashboard đôi khi ghi nhận lệnh khác vào vùng điều khiển quá sớm hoặc một tác vụ đang chạy báo exception khi kết thúc.

## Bạn cần làm gì
1. Chạy starter, tái hiện hiện tượng.
2. Thu thập trạng thái `canceled`, `overlap`, `releaseFault`, `firstCompleted`.
3. Đưa ít nhất hai giả thuyết; xác định thời điểm một tác vụ thực sự có quyền truy cập tài nguyên.
4. Sửa **starter/IrrigationController.cs**, giữ nguyên public API và không sửa harness.
5. Chạy verify và kiểm tra cả đường thành công lẫn đường hủy.

## Yêu cầu môi trường
.NET SDK 8.0.x, PowerShell 7 hoặc Windows PowerShell; không cần cloud, database hay NuGet ngoài SDK.

## Chạy nhanh
Từ thư mục unit:
```powershell
dotnet --version
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
`./reproduce.ps1` chạy một lệnh đang giữ tài nguyên, hủy lệnh đang chờ rồi đưa thêm lệnh thứ ba vào. Harness báo `REPRODUCED` khi ít nhất một hợp đồng điều phối bị vi phạm. Script này chỉ dùng để xác nhận trạng thái starter ban đầu.

## Những gì cần quan sát
Đối chiếu thứ tự các lệnh với trạng thái cancellation và exception; một lệnh bị hủy không được thực thi action. Một lệnh khác không được thực thi khi bộ điều khiển vẫn bận.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trên starter.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp và trade-offs](solution/README.md).

## Expected Results
Trước sửa: reproduce exit 0 với dấu hiệu vi phạm; verify exit khác 0. Sau sửa: verify exit 0, không overlap, không exception cleanup và action bị hủy không chạy. Không dùng thời gian tuyệt đối để đánh giá hiệu năng.

## Estimated Time
35–45 phút.
