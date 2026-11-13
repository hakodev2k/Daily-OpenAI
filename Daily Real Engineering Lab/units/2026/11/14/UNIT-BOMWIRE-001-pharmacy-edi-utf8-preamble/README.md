# UNIT-BOMWIRE-001 — EDI dược phẩm: file UTF-8 nhìn đúng nhưng gateway từ chối

## Mục tiêu
Điều tra khác biệt giữa nội dung text hiển thị và byte-level contract; dùng bằng chứng trước khi sửa.

## Bối cảnh thực tế
Nhà phân phối gửi manifest EDI tới gateway nhà thuốc. Sau khi nâng cấp exporter, file mở trong editor vẫn có dòng đầu `HDR|20261114|PHARMA`, nhưng gateway báo `INVALID_HEADER_OFFSET_0`. Các dòng tiếp theo vẫn đọc được.

## Bạn cần làm gì
Chạy exporter, thu thập byte đầu, so sánh với contract, đưa ra ít nhất hai giả thuyết, sửa exporter và chứng minh không làm hỏng tiếng Việt.

## Yêu cầu môi trường
.NET SDK 8.0, PowerShell 7+ hoặc shell hỗ trợ `dotnet`. Không cần mạng hoặc dịch vụ ngoài.

## Chạy nhanh
```powershell
dotnet run --project starter/PharmacyEdi.csproj -- export
dotnet run --project starter/PharmacyEdi.csproj -- inspect
```

## Cách reproduce vấn đề
Từ thư mục unit chạy `./reproduce.ps1` hoặc hai lệnh trên. Ghi lại exit code và 12 byte đầu tiên. Nếu dùng Linux/macOS, chạy trực tiếp `dotnet run`.

## Những gì cần quan sát
Gateway mô phỏng yêu cầu ba byte đầu đúng `48 44 52` (ASCII `HDR`). Đầu ra hiện tại bị từ chối dù editor hiển thị header. Xem cả các byte của dòng tên sản phẩm có dấu.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis và evidence vào `investigation.md`.
3. Thử fix trong `starter/Program.cs`.
4. Verify bằng `./verify.ps1` hoặc `dotnet run --project starter/PharmacyEdi.csproj -- verify`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/01.md)
- [Hint 2](hints/02.md)

## Reference Solution
**Spoiler:** [Phân tích và patch](solution/README.md)

## Expected Results
Sau sửa: `verify` trả exit code 0, header bắt đầu đúng offset 0, tên sản phẩm có dấu giữ nguyên, file không có byte thừa trước header.

## Estimated Time
40 phút.
