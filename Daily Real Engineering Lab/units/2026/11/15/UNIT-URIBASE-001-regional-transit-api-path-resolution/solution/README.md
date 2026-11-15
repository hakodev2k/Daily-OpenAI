# Reference solution (spoiler)

## Root cause
`new Uri(baseAddress, relative)` áp dụng RFC 3986 URI reference resolution: `/fares/quote` thay thế toàn bộ base path; `../alerts` đi lên một segment. Đây không phải phép nối chuỗi path.

## Contract
Partner endpoint phải là path tương đối với API prefix; loại bỏ leading slash chỉ khi được business contract cho phép, và reject parent traversal, absolute URL, encoded delimiters. Không cho input điều khiển origin hoặc thoát `/api/v2/`.

## Fix và trade-offs
`solution/Program.cs` minh họa normalize và prefix containment; với production, dùng endpoint identifiers được khai báo tĩnh và URI builder thay vì nhận raw arbitrary path. Ví dụ fixture `../alerts` được đổi thành `alerts` trong contract v2; không thể sửa bằng normalize nếu đối tác thực sự yêu cầu một path ngoài prefix.

## Wrong fixes
- Chỉ tăng HttpClient.Timeout: 404 không phải timeout.
- Dùng string concatenation không encode/validate: tạo URL không hợp lệ hoặc vượt trust boundary.
- Tắt kiểm tra host và redirect: mở rộng surface SSRF.
- Dùng TrimStart('/') mà không chặn `..`: vẫn thoát API prefix.

## Verification
Chạy `scripts/verify.ps1` sau khi sửa starter. Static review; chưa chạy dotnet trong môi trường xuất bản.
