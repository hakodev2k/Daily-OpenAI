# Reference Solution

> Chỉ đọc sau khi bạn đã reproduce và tự thử sửa.

## 1. Symptoms

Middleware log được raw JSON, nhưng downstream endpoint không bind được webhook body và request thất bại trước khi business handler xử lý dữ liệu.

## 2. Evidence

Cùng một `HttpRequest.Body` được middleware đọc trước endpoint. Sau `ReadToEndAsync()`, vị trí đọc đã ở cuối stream. Downstream model binding không nhận được payload ban đầu.

## 3. Root cause

Request body mặc định là một forward-oriented stream trong request pipeline. Middleware đã tiêu thụ body nhưng không thiết lập cơ chế đọc lại và không đưa stream về vị trí ban đầu trước khi gọi `next`.

## 4. Why the fix works

`EnableBuffering()` cho phép request body có thể được đọc lại. Middleware dùng `StreamReader(..., leaveOpen: true)` để không đóng stream thuộc sở hữu của request, sau đó đặt `Request.Body.Position = 0` trước khi chuyển control xuống downstream pipeline.

## 5. How to verify

Chạy:

```powershell
./verify.ps1
```

Verification chỉ pass khi chính code trong `starter/` trả `200 OK` và response chứa `ORD-42`.

## 6. Alternative fixes

- Nếu chỉ một endpoint cần raw body, có thể giới hạn middleware theo path để giảm buffering không cần thiết.
- Với signature verification, có thể thiết kế endpoint/filter chuyên biệt thay vì global middleware nếu scope nhỏ hơn.
- Nếu payload rất lớn, cần cân nhắc giới hạn body size, buffering threshold và disk spill behavior.

## 7. Wrong / tempting fixes

### Đổi DTO hoặc JSON options

Không giải quyết việc downstream không còn bytes để deserialize.

### Đọc body lần hai trong endpoint

Nếu upstream đã tiêu thụ body và không rewind, lần đọc thứ hai vẫn không phục hồi payload.

### Copy raw payload vào `HttpContext.Items` rồi bỏ model binding

Có thể hoạt động trong một thiết kế khác, nhưng đây là thay đổi contract/pipeline lớn hơn cần thiết và dễ làm business handler phụ thuộc middleware implementation.

### Hard-code payload hoặc retry request

Che triệu chứng chứ không sửa ownership/lifecycle của request stream.

## 8. Production implications

Buffering có chi phí memory/I/O. Không bật global một cách vô điều kiện cho payload lớn. Signature validation cũng cần thực hiện trên raw bytes chính xác trước khi parse nếu provider contract yêu cầu như vậy.

## 9. Trade-offs

Giải pháp buffering + rewind đơn giản và phù hợp cho webhook payload nhỏ. Với upload lớn hoặc streaming request, việc buffer toàn bộ body có thể phá mục tiêu streaming và cần kiến trúc khác.

## 10. Senior engineer should notice

- Middleware có thể thay đổi observable state của downstream pipeline.
- Stream ownership quan trọng: component đọc stream không đồng nghĩa component đó được phép đóng stream.
- Fix phải bảo toàn cả audit requirement lẫn endpoint behavior.
- Cần xem xét payload limits và resource usage trước khi áp dụng buffering toàn cục.
