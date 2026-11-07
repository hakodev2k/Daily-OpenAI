# Reference Solution — SPOILER: chỉ xem sau khi tự điều tra

## 1. Symptoms

Client tải toàn bộ waveform đúng nhưng các partial response có payload ngắn hơn metadata mô tả. `206` được ghi nhận như thành công dù dữ liệu thiếu mẫu.

## 2. Evidence

Harness có `200 ABCDEFGHIJ`, trong khi `Range: bytes=3-6` trả `206`, `Content-Range: bytes 3-6/10` nhưng body `DEF` thay vì `DEFG`. `bytes=0-0` có body rỗng. Cả hai là bằng chứng về độ dài đoạn dữ liệu, không phải lỗi mạng.

## 3. Root cause

HTTP byte range dùng chỉ số cuối **inclusive**. Trong starter, `Waveform.AsMemory(start, end - start)` truyền tham số thứ hai là length nhưng lại tính như thể end exclusive. Mọi đoạn hợp lệ bị thiếu một byte cuối, kể cả đoạn một byte thành rỗng.

## 4. Why the fix works

[WaveformEndpoint.cs](WaveformEndpoint.cs) sử dụng `Waveform.AsMemory(start, checked(end - start + 1))`. Hai biên đã được chuẩn hóa và kiểm tra trước đó; độ dài tương ứng với Content-Range. `checked` thể hiện chủ đích kiểm soát arithmetic, dù fixture 10 byte không có nguy cơ overflow.

## 5. How to verify

Tự sửa `starter/WaveformEndpoint.cs` rồi chạy `./scripts/verify.ps1`. Test gọi HTTP server thực trên loopback, đối chiếu status, body và Content-Range cho full, bounded, one-byte, open-end, suffix và unsatisfiable. Không sửa `starter/Program.cs` để làm xanh test. Có thể so sánh implementation với file reference sau đó.

## 6. Alternative fixes

- Với file tĩnh hoặc representation ổn định, cân nhắc `Results.File(..., enableRangeProcessing: true)` của ASP.NET Core thay vì tự triển khai protocol.
- Một helper `GetRangeLength(start, endInclusive)` có contract rõ ràng sẽ giảm lỗi chuyển đổi giữa index và length.
- Nếu API không cần partial download, bỏ hỗ trợ Range và trả representation đầy đủ là lựa chọn đơn giản hơn, nhưng thay đổi khả năng resume/stream của client.

## 7. Wrong / misleading fixes

- Tăng HTTP timeout: không ảnh hưởng đến số byte được cắt.
- Tự thêm byte ở client: che lỗi, phá tương thích với endpoint đúng.
- Trả `200` cho mọi Range để tránh kiểm tra: có thể phù hợp khi chủ động không hỗ trợ Range, nhưng không phải sửa lỗi nếu sản phẩm cam kết partial download.
- Chỉ sửa `Content-Range` cho khớp body ngắn: hợp thức hóa dữ liệu thiếu thay vì phục hồi contract.

## 8. Production implications

Response 206 phải nhất quán giữa payload, Content-Range, representation length và validators. Với file có thể thay đổi, xem xét ETag/`If-Range` để tránh ghép các phiên bản khác nhau. Với tải nhiều đoạn, cần xử lý multipart hoặc cố ý bỏ qua Range không hỗ trợ theo HTTP semantics. Thêm telemetry cho mismatch và contract tests ở ranh giới 0, EOF, range ngoài phạm vi.

## 9. Trade-offs

Tự xử lý Range giúp kiểm soát dữ liệu sinh động nhưng tăng trách nhiệm protocol, security và test coverage. Dùng framework file results giảm lỗi triển khai nhưng không phải lúc nào phù hợp với dữ liệu sinh theo yêu cầu. Trả 200 đơn giản nhưng tốn băng thông và làm mất khả năng resume.

## 10. What a Senior engineer should notice

Status 2xx chỉ nói request đã được xử lý theo quan điểm server, không chứng minh payload đúng. Đặt contract tests ở biên HTTP, dùng fixture xác định, kiểm tra byte-level integrity và cân nhắc thay đổi representation giữa các lần tải. Không tăng User Mastery chỉ vì lab được phát hành.

## Tài liệu chính thức

- [RFC 9110 — Range Requests](https://www.rfc-editor.org/rfc/rfc9110.html#section-14)
- [RFC 9110 — Content-Range](https://www.rfc-editor.org/rfc/rfc9110.html#section-14.4)
- [ASP.NET Core Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-8.0)
