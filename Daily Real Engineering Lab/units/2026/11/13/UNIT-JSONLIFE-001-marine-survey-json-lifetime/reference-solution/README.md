# Reference Solution — spoiler
## Symptoms
Parse thành công, downstream ném ObjectDisposedException.
## Evidence
Stack trace chỉ vào GetProperty sau khi method Read đã return; network log là nhiễu.
## Root cause
JsonElement giữ tham chiếu đến JsonDocument đã dispose khi rời using.
## Why fix works
Clone tạo element độc lập trước khi document dispose.
## How to verify
Thay starter/SurveyImport.cs bằng code reference rồi chạy pwsh ./verify.ps1; kiểm tra hai payload.
## Alternative fixes
Deserialize thành DTO bất biến; hoặc chuyển ownership của JsonDocument sang caller kèm dispose contract.
## Wrong fixes
Bỏ using làm tăng rủi ro giữ buffer; retry không khôi phục lifetime; tăng timeout không xử lý lỗi.
## Production implications
Định nghĩa rõ ownership qua service boundary và test downstream.
## Trade-offs
Clone cấp phát thêm; DTO tăng type safety; giữ document sống đòi hỏi quản lý lifetime.
## Senior engineer should notice
Lỗi downstream không nhất thiết do JSON input; resource lifetime là contract liên tầng.
