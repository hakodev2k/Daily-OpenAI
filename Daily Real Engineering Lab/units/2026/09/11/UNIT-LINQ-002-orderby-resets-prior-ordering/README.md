# UNIT-LINQ-002 — OrderBy Resets Prior Ordering

## Mục tiêu

Điều tra một pipeline LINQ tạo danh sách export có đủ dữ liệu nhưng thứ tự business priority không còn được giữ đúng.

## Bối cảnh thực tế

Một job tạo danh sách invoice để gửi sang hệ thống thu hồi công nợ. Yêu cầu nghiệp vụ là invoice có `Priority` thấp hơn phải đứng trước; trong cùng một `Priority`, invoice có `DueDate` sớm hơn đứng trước. Sau một refactor nhỏ, file export vẫn chứa đủ invoice nhưng thứ tự bị thay đổi và downstream bắt đầu xử lý invoice ưu tiên thấp trước invoice ưu tiên cao.

## Bạn cần làm gì

1. Chạy starter và reproduce thứ tự hiện tại.
2. Ghi hypothesis về cách LINQ đang xây dựng ordering.
3. Sửa trực tiếp code trong `starter/` để đáp ứng đúng contract sắp xếp.
4. Chạy `verify.ps1` để kiểm tra learner-editable code.
5. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell

## Chạy nhanh

```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Có đúng 3 invoice trong output.
- `ORDER` hiện tại là `INV-C,INV-B,INV-A`.
- Contract mong muốn là `INV-C,INV-A,INV-B`.
- Không có exception; đây là lỗi semantic trong kết quả.

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

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

- [Reference Solution](solution/README.md)

## Expected Results

Before:
- `ORDER=INV-C,INV-B,INV-A`
- `reproduce.ps1` pass vì symptom được tái hiện đúng

After:
- `ORDER=INV-C,INV-A,INV-B`
- `verify.ps1` pass

Nếu không reproduce được, chạy `dotnet --info`, xác nhận .NET 8 SDK có sẵn rồi chạy lại từ chính thư mục unit.

## Estimated Time

25–40 phút.
