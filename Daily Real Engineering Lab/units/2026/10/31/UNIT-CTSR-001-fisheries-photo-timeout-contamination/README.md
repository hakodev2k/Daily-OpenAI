# UNIT-CTSR-001 — Ảnh cập cảng bị timeout dây chuyền

## Mục tiêu
Reproduce lỗi theo chuỗi yêu cầu, thu thập bằng chứng, xác định phạm vi lifecycle, sửa và verify cả timeout lẫn caller cancellation.

## Bối cảnh thực tế
Ứng dụng duyệt ảnh cập cảng dùng một instance service cho nhiều ảnh. Ảnh đầu gọi đối tác chậm; hai ảnh sau được đối tác trả lời ngay. Tuy vậy, hệ thống đánh dấu cả ba là timeout.

## Bạn cần làm gì
Chạy starter, viết ít nhất hai giả thuyết, dùng debugger quan sát tín hiệu hủy của từng ảnh, sửa `starter/PhotoInspectionService.cs` và giữ nguyên test oracle trong `Program.cs`.

## Yêu cầu môi trường
PowerShell 5.1+ hoặc 7+, .NET SDK 8+ hỗ trợ target `net8.0`. Không cần cloud, Docker hay database.

## Chạy nhanh
Từ thư mục unit:

    ./setup.ps1
    ./reproduce.ps1
    ./verify.ps1

`reproduce.ps1` PASS trên starter lỗi; `verify.ps1` FAIL trước khi sửa và phải PASS sau khi sửa.

## Cách reproduce vấn đề
Chạy `./run.ps1` để quan sát ba quyết định liên tiếp; hoặc chạy `dotnet run --project starter/CoastalInspection.csproj -- reproduce`. Ghi output vào `workspace/my-investigation.md`.

## Những gì cần quan sát
So sánh kết quả ảnh chậm và hai ảnh nhanh. Phân biệt lỗi downstream thực sự với trạng thái tồn tại giữa các lần gọi. Đảm bảo caller chủ động hủy không bị phân loại thành partner timeout.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Reference và trade-offs](solution/README.md). Chỉ mở sau khi tự thử sửa.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time
60–75 phút.
