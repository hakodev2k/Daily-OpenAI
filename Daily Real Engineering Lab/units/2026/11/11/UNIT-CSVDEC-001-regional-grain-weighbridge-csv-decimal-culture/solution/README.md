# Reference Solution — chỉ đọc sau khi tự điều tra

## Symptoms / Evidence
Cùng CSV `WB-492,1250.75` chạy khác nhau giữa `en-US` và `de-DE`/`vi-VN`. Lưu stdout/stderr của `reproduce.ps1` để so sánh.

## Root cause
`decimal.TryParse(string, out decimal)` dùng `CurrentCulture`. Chuỗi dot-decimal theo wire contract không thể diễn giải an toàn khi culture máy thay đổi; ở một số culture dấu chấm là dấu phân nhóm, có thể parse sai thay vì báo lỗi.

## Why the fix works
Chỉ định rõ `NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint` cùng `CultureInfo.InvariantCulture` ở ranh giới nhập dữ liệu; không cho phép dấu nhóm số. Không thay culture toàn bộ process.

## How to verify
Thay `starter/Program.cs` bằng reference rồi chạy `./verify.ps1`; kết quả mọi culture đều `kilograms=1250.75`. Chạy thêm trường hợp payload `1,250.75` phải bị từ chối nếu partner không cho thousands separator.

## Alternative fixes
Dùng parser CSV có schema culture cố định nếu nhiều trường và quoting phức tạp; chọn `NumberStyles.Number` chỉ khi hợp đồng thực sự cho phép dấu nhóm.

## Wrong / tempting fixes
Đổi `CurrentCulture` toàn ứng dụng: có thể ảnh hưởng định dạng người dùng, logs và thư viện khác. Thay dấu `,` bằng `.` trên mọi input: phá số có phân nhóm và không xử lý ambiguity. Dùng `double`: không giải quyết ranh giới culture và có thêm sai số nhị phân.

## Production implications / trade-offs
Cố định schema input, kiểm thử đa culture trong CI, báo lỗi có ticket ID nhưng không log dữ liệu nhạy cảm; quy định rõ separator, scale và range hợp lệ. Senior engineer phân biệt wire contract với locale hiển thị UI.

## Reference source
[Program.cs](Program.cs)
