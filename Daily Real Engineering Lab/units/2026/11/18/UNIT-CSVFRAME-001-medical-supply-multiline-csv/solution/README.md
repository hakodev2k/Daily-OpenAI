# Reference Solution — chỉ đọc sau khi tự điều tra

## Symptoms & Evidence
Dữ liệu có 2 bản ghi nhưng chương trình ban đầu có thể bỏ qua dòng và cắt nội dung ghi chú. Contract check thất bại dù không có exception.

## Root cause
CSV có **logical record** khác physical line: newline nằm trong quoted field là dữ liệu, không phải record delimiter. Ngoài ra `""` biểu diễn dấu ngoặc kép, dấu phẩy trong quoted field không tách cột.

## Why the fix works
Parser dùng state `quoted`, chỉ nhận dấu phẩy/newline là delimiter khi ở ngoài quoted field, xử lý escaped quotes, kiểm tra header, số cột và dữ liệu sai định dạng. Không âm thầm bỏ dòng.

## How to verify
Đối chiếu `starter/Program.cs` với reference, áp dụng sửa vào starter rồi chạy `./verify.ps1`. `verify.ps1` **không** chạy source trong solution.

## Alternative fixes
Thư viện CSV được bảo trì tốt như CsvHelper phù hợp production nếu chính sách cho phép thêm dependency, cấu hình rõ dialect và encoding; cần pin version và test với dữ liệu đối tác.

## Wrong / tempting fixes
`Trim()` toàn dòng làm mất dữ liệu hợp lệ; `Split(',')` vẫn hỏng khi field chứa dấu phẩy; thay `\r\n` bằng khoảng trắng trước parse phá vỡ note; bỏ qua dòng sai khiến báo cáo thiếu hàng mà không báo lỗi.

## Production implications & trade-offs
Cần streaming parser cho file lớn, giới hạn kích thước field/record, audit số dòng đầu vào/đầu ra, reject/quarantine toàn batch hoặc từng record theo hợp đồng nghiệp vụ. Parser tự viết có chi phí bảo trì cao hơn thư viện đã kiểm chứng.

## Senior engineer should notice
Tính toàn vẹn dữ liệu quan trọng hơn việc import không throw; đo row count, reject rate, reconciliation và test malformed quoting, CRLF/LF, encoding, duplicate SKU trước khi release.