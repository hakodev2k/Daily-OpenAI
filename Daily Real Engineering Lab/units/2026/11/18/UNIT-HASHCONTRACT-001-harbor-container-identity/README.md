# UNIT-HASHCONTRACT-001 — Container identity bị trùng khi nhập manifest cảng

## Mục tiêu
Điều tra sai lệch deduplication của manifest container và chứng minh tính đúng đắn của collection contract trong .NET.

## Bối cảnh thực tế
Cảng tiếp nhận manifest từ hai hãng tàu. Cùng một container có thể được nhập với chữ hoa/chữ thường khác nhau. Dashboard báo 3 container trong khi nghiệp vụ kỳ vọng 2; tra cứu theo mã đối tác đôi khi không thấy bản ghi.

## Bạn cần làm gì
Chạy starter, lưu output, đặt ít nhất hai giả thuyết, kiểm tra điều kiện identity, sửa trong starter và xác nhận số lượng/lookup đều đúng.

## Yêu cầu môi trường
.NET SDK 8 trở lên, PowerShell 7 hoặc Windows PowerShell; không cần database hay dịch vụ ngoài.

## Chạy nhanh
Từ thư mục unit: `dotnet run --project starter/HarborManifest.csproj`.

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1` hoặc `dotnet run --project starter/HarborManifest.csproj -- --reproduce`. Ghi lại ba trường equivalent, hashEqual và unique.

## Những gì cần quan sát
Đối chiếu số lượng manifest đầu vào, số container nghiệp vụ, kết quả lookup theo mã khác kiểu chữ và tính ổn định khi đổi thứ tự input. Không giả định một kết quả đúng chỉ vì chương trình không throw exception.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ mở [solution/README.md](solution/README.md) sau khi tự sửa và verify.

## Expected Results
Trước sửa: identity không nhất quán; sau sửa: hai container duy nhất và lookup theo mã tương đương thành công.

## Estimated Time
35–45 phút.
