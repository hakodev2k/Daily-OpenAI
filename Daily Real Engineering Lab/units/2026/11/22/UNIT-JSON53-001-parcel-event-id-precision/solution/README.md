# Reference Solution — chỉ xem sau khi tự sửa

## Symptoms
Ba JSON event ID khác nhau, chỉ hai event được nhận.

## Evidence
Log ghi `DUPLICATE key=9007199254740992` cho parcel P-102; payload chứa `9007199254740993`.

## Root cause
`JsonElement.GetDouble()` chuyển JSON integer sang IEEE-754 double. 9007199254740993 vượt giới hạn nguyên liên tiếp biểu diễn chính xác của double (2^53), bị làm tròn thành 9007199254740992. Chuỗi khóa sau đó collision. Đây là lỗi precision trước dedup, không phải lỗi Dictionary.

## Why the fix works
`GetInt64()` đọc số nguyên trong miền signed 64-bit mà không qua floating-point; `ToString(InvariantCulture)` tạo identity ổn định.

## How to verify
`dotnet run --project solution/ParcelEvents.csproj` trả 0. Áp dụng thay đổi tương đương trong `starter/Program.cs`, chạy `pwsh -File scripts/verify.ps1`.

## Alternative fixes
Dùng `GetRawText()` nếu contract xác nhận canonical numeric tokens, nhưng phải xử lý các biểu diễn như 1e3 so với 1000. Dùng string ID từ upstream nếu ID không thực sự là số cần tính toán hoặc vượt Int64.

## Wrong / tempting fixes
Tăng dung lượng Dictionary không thay đổi collision. Retry cùng payload tiếp tục mất event. Thêm GUID vào key làm hỏng dedup/idempotency. Dùng decimal có thể phù hợp một số contract nhưng cần định nghĩa range và parsing rõ ràng.

## Production implications
Theo dõi count accepted/rejected, lưu raw partner ID trong log có giới hạn và không log PII, kiểm thử các biên 2^53 và Int64.MaxValue, thống nhất JSON schema và versioning với đối tác.

## Trade-offs
Int64 chặt chẽ, nhanh nhưng không hỗ trợ số nguyên ngoài phạm vi; string linh hoạt nhưng đòi hỏi chuẩn hóa rõ ràng.

## Senior engineer should notice
Chứng minh invariant identity tại ingestion boundary, không chẩn đoán nhầm persistence hay concurrency khi evidence cho thấy collision trước khi ghi.
