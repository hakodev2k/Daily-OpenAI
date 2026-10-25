# Investigation guide
1. Chạy smoke trước để xác nhận đường 202 bình thường.
2. Chạy reproduce; ghi exception type và partner_calls.
3. Đọc luồng control flow giữa lần trả 503 và attempt kế tiếp. Liệt kê các object được giữ lại.
4. Đưa ra hai hypothesis cạnh tranh: partner trả payload lỗi, serialization lỗi, hoặc client không thực hiện lần gửi mới.
5. Đề xuất thay đổi tối thiểu, không đổi hợp đồng POST transfers và retry budget.
6. Chạy verify và lưu kết quả; chỉ sau đó xem reference solution.

## Evidence cần ghi
| Test | Partner calls | Client outcome | Body/key |
|---|---:|---|---|
| 202 | | | |
| 503→202 | | | |
| 400 | | | |
| 503→503 | | | |

## Troubleshooting
Nếu không có .NET SDK 8+, cài SDK phù hợp; nếu PowerShell bị chặn execution policy, dùng dotnet run --project starter/TransferLab.csproj -- --mode reproduce/verify. Fake handler không gọi internet.
