# Reference Solution — SPOILER

**Chỉ đọc sau khi reproduce và tự thử sửa.**

## Symptoms
CSV có đủ 8 đăng ký, nhưng sáu cell có nội dung dễ bị spreadsheet xem như công thức.

## Evidence
Reproduce ghi RiskyCells=6; CSV parser vẫn parse đúng các field có dấu phẩy, ngoặc kép và newline. HTTP 200 không phản ánh hành vi importer.

## Root cause
CSV quoting chỉ bảo vệ cú pháp phân tách field, không định nghĩa cách spreadsheet phân loại nội dung cell. Input bên ngoài đi qua trust boundary mà không có chính sách dành cho spreadsheet.

## Why the fix works
solution/RosterCsv.cs kiểm tra các tiền tố =, +, -, @, tab, CR/LF và khoảng trắng dẫn đến tiền tố nguy cơ; thêm apostrophe trước khi CSV-quote. Áp dụng cho mọi field không đáng tin. Đây là chính sách bảo thủ cho fixture và consumer dạng spreadsheet, không phải cam kết an toàn cho mọi ứng dụng.

## How to verify
Chạy ./verify.ps1 trên starter đã sửa (không phải solution). Đối chiếu riêng reference bằng:
    dotnet run --project ./solution/RosterLab.csproj -- verify

## Alternative fixes
XLSX với explicit string cell types và kiểm thử trên client mục tiêu là lựa chọn mạnh hơn. Nếu consumer là máy, dùng canonical CSV contract riêng và kiểm soát import thay vì tự ý thay đổi dữ liệu.

## Wrong or misleading fixes
- Chỉ CSV-quote: vẫn có nguy cơ công thức.
- Chỉ chặn dấu =: bỏ sót các tiền tố khác.
- Chỉ lọc Comment: các cột khác cũng là input.
- Xóa toàn bộ ký tự đặc biệt: phá dữ liệu hợp lệ.
- Chỉ kiểm tra số dòng: bỏ qua semantics của cell.

## Production implications
Định nghĩa export purpose và spreadsheet client được hỗ trợ. Kiểm tra thực tế import/save/reopen, tránh log PII, thêm regression fixture. Apostrophe prefix có thể xuất hiện trong một số client và làm thay đổi raw value.

## Trade-offs
Chính sách bảo thủ cũng neutralize chuỗi số âm; phù hợp với text export nhưng không nên áp dụng mù quáng cho dữ liệu máy-máy. XLSX tăng chi phí triển khai nhưng cung cấp cell typing rõ ràng.

## Senior engineer should notice
Phải kiểm soát trust boundary và mục đích sử dụng, không chỉ syntax; bảo toàn field, định nghĩa acceptance test theo client, và phân tách export contract.

**Không thực thi bất kỳ công thức nào trong lab này.**
