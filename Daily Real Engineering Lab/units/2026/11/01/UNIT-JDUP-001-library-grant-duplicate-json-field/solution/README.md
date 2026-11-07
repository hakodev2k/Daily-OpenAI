# Reference Solution — SPOILER

Chỉ đọc sau khi đã reproduce và tự thử fix.

## 1. Symptoms

Yêu cầu `LIB-204` được duyệt tự động nhưng sổ ghi 42.000, vượt giới hạn 5.000. Đây là sai lệch **integrity**, không phải vấn đề hiệu năng.

## 2. Evidence

Raw payload chứa hai property `grantAmount`. Intake dùng `EnumerateObject().First(...)`, đọc 4.200. Accounting dùng `GetProperty("grantAmount")`, trong `System.Text.Json` trả property cuối cùng khi tên bị lặp, đọc 42.000. Hai thành phần đã diễn giải một message thành hai giá trị khác nhau.

## 3. Root cause

JSON có tên property trùng lặp, và hai bước xử lý không dùng chung một biểu diễn request đã được xác thực. Kiểm tra giới hạn trên một giá trị không ràng buộc giá trị thực tế ghi sổ.

## 4. Why the fix works

[Program.cs](Program.cs) duyệt object đúng một lần, phát hiện duplicate bằng `HashSet<string>(StringComparer.Ordinal)`, từ chối unknown/invalid fields và chỉ dùng **một giá trị amount đã validate** cho cả quyết định duyệt và kết quả ghi sổ. Không có bước parse thứ hai từ dữ liệu chưa tin cậy.

## 5. How to verify

Sau khi so sánh, có thể chép reference vào `starter/Program.cs` để tự kiểm chứng, nhưng hãy ưu tiên giữ bản fix riêng của bạn:

```powershell
./scripts/verify.ps1
```

Contract runner ở `checks/` tham chiếu `starter/`. Cần tất cả 13 cases PASS. `reproduce.ps1` chỉ dùng để chứng minh **starter gốc** có lỗi, không phải post-fix test.

## 6. Alternative fixes

- Reject duplicate properties ngay tại API gateway, sau đó parse thành DTO immutable có schema strict: phù hợp nếu toàn bộ downstream đều dùng DTO đã xác thực.
- Dùng một JSON schema validator được cấu hình phát hiện duplicate: cần kiểm chứng cụ thể behavior của parser/validator; schema validation thông thường không bảo đảm phát hiện duplicate nếu parser đã làm mất thông tin.

## 7. Wrong / tempting fixes

- Chỉ thay `First` bằng `GetProperty`: hai bước có thể nhất quán nhưng vẫn chấp nhận payload mơ hồ; không giải quyết trust boundary.
- Chỉ đổi giới hạn hoặc log cảnh báo: không ngăn ghi sổ sai.
- Dùng `Dictionary` ghi đè các key trùng: lặng lẽ chọn giá trị cuối; không thể hiện chính sách reject.
- Dựa vào test chỉ có JSON hợp lệ: bỏ sót đường tấn công và lỗi dữ liệu.

## 8. Production implications

Reject malformed/ambiguous requests bằng 400, ghi structured error code và correlation ID nhưng không log toàn bộ payload nhạy cảm. Ràng buộc authorization, approval và persistence trong cùng một validated command; khi dùng database, kiểm tra transaction boundary và audit record. Tránh việc downstream tự parse raw payload theo chính sách khác.

## 9. Trade-offs

Strict allowlist từ chối unknown fields, có thể ảnh hưởng tương thích API. Khi cần mở rộng schema, version contract và thử compatibility trước; duplicate property vẫn nên bị reject. `StringComparer.Ordinal` quy định tên JSON phân biệt hoa/thường; nếu API dùng case-insensitive mapping, phải phát hiện cả collisions theo chính sách đó.

## 10. Senior engineer nên nhận ra

Một request không có canonical interpretation là một lỗ hổng hợp đồng giữa service. Bài toán không chỉ là sửa một API call mà là bảo đảm **parse once → validate once → authorize once → persist exactly validated data**, với negative contract tests.
