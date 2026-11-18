# UNIT-DICTKEY-001 — Mã thiết bị biến mất khỏi Dictionary sau cập nhật

## Mục tiêu
Điều tra lỗi tra cứu registry trong .NET, phân biệt identity và dữ liệu nghiệp vụ thay đổi.

## Bối cảnh thực tế
Hệ thống bảo trì thiết bị lưu trạng thái inspection. Khi nhân viên đổi nhãn thiết bị, API vẫn báo có một bản ghi nhưng không thể tra cứu hoặc xóa bản ghi đó.

## Bạn cần làm gì
Chạy bản starter, ghi nhận bằng chứng, nêu tối thiểu hai giả thuyết, sửa thiết kế và chứng minh thao tác tra cứu/xóa đúng sau cập nhật.

## Yêu cầu môi trường
.NET SDK 8 trở lên, PowerShell; không cần dịch vụ ngoài.

## Chạy nhanh
Từ thư mục unit: `dotnet run --project starter/Registry.csproj`.

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` trong PowerShell. Nếu policy chặn script, chạy `powershell -ExecutionPolicy Bypass -File ./reproduce.ps1`.

## Những gì cần quan sát
Ghi lại giá trị before, after, remove, count; phân biệt số lượng lưu trữ và khả năng truy xuất. Không suy luận nguyên nhân chỉ từ count.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp và trade-offs](solution/README.md).

## Expected Results
Starter cho thấy lookup/removal bất thường; bản sửa phải trả về before=True, after=True, remove=True, count=0.

## Estimated Time
35 phút.
