# Reference Solution — SPOILER

Chỉ xem sau khi đã chạy reproduce, ghi hypothesis và thử sửa `starter/ManifestIntake.cs`.

## 1. Symptoms
Tờ khai có `requiresInspection: true` ở phần đầu payload nhưng quyết định sau ingress trở thành `false`, không có parse error. Đường dữ liệu hợp lệ vẫn hoạt động bình thường.

## 2. Evidence
So sánh raw payload với kết quả `ManifestIntake.Parse`. Một JSON object có cả `requiresInspection` và `RequiresInspection` mang giá trị trái ngược. Với `PropertyNameCaseInsensitive=true`, hai tên cùng ánh xạ vào một CLR property; giá trị xuất hiện sau ghi đè giá trị trước. Dòng `AMBIGUOUS_ACCEPTED=True` và `INSPECTION_DOWNGRADED=True` xác nhận hiện tượng. Log nghiệp vụ chỉ ghi quyết định cuối nên không đủ để suy luận dữ liệu nguồn.

## 3. Root cause
JSON cho phép parser gặp nhiều property có cùng tên; hợp đồng nghiệp vụ lại không định nghĩa cách xử lý các tên tương đương theo chính sách case-insensitive. `System.Text.Json` không mặc định từ chối kiểu xung đột này. Parse thành công không có nghĩa payload không mơ hồ.

## 4. Why the fix works
Trước khi materialize DTO, duyệt `JsonDocument.RootElement.EnumerateObject()` và lưu tên trong `HashSet<string>(StringComparer.OrdinalIgnoreCase)`. Nếu cùng logical name xuất hiện hai lần, throw `JsonException`. Sau đó mới `Deserialize<ClearanceDeclaration>` với chính sách case-insensitive hiện có. Kiểm tra top-level phù hợp contract hiện tại (DTO phẳng); với nested DTO phải xác định rõ phạm vi validate.

## 5. How to verify
Chạy `./verify.ps1` trên starter do bạn chỉnh. Sau khi tự làm, có thể so sánh reference:

```powershell
Copy-Item ./starter/ManifestIntake.cs ./starter/ManifestIntake.mine.cs.bak
Copy-Item ./solution/ManifestIntake.cs ./starter/ManifestIntake.cs -Force
./verify.ps1
```

Harness đòi 8 checks: 4 payload hợp lệ (bao gồm giá trị `false` hợp pháp, khác casing và extension field) và 4 negative paths (hai dạng trùng tên, root không phải object, JSON sai cú pháp). Không sửa `Program.cs` để làm tests pass.

## 6. Alternative fixes
- Strict canonical property naming + reject unknown fields là chính sách mạnh hơn, nhưng có thể phá backward compatibility với đối tác cũ.
- Dùng streaming `Utf8JsonReader` để kiểm tra trùng tên khi payload lớn, nhưng cần quản lý depth, object scope và resource limits; không nên tự viết parser phức tạp khi không cần.
- Reject chỉ các tên field nghiệp vụ quan trọng có thể giảm breaking changes, nhưng phải định nghĩa allowlist rõ ràng và dễ bỏ sót field mới.

## 7. Wrong / tempting fixes
- Đổi `PropertyNameCaseInsensitive=false` đơn thuần: có thể làm một key bị ignore thay vì reject, đồng thời phá compatibility.
- Lấy giá trị đầu tiên hoặc cuối cùng theo quy ước ngầm: vẫn cho phép payload mơ hồ và khác hành vi giữa các hệ thống.
- Log thêm quyết định cuối: không bảo đảm phát hiện xung đột ở ingress.
- Sửa giá trị `false` thành `true` mặc định: phá các tờ khai hợp lệ không cần kiểm tra.
- Bỏ `JsonException` và trả 200: tiếp tục che khuất dữ liệu sai contract.

## 8. Production implications
Đặt giới hạn kích thước request, số trường và nesting theo threat model; tránh log raw payload có PII. Trả 400 kèm error code ổn định cho payload mơ hồ. Quan sát số lượng reject theo partner, test qua gateway và nhiều ngôn ngữ, đặc biệt khi có serializer khác. Không coi static-review là kết quả build/test thực tế.

## 9. Trade-offs
`JsonDocument` giúp code đơn giản và chính xác với DTO nhỏ nhưng tạo DOM và giữ bộ nhớ theo kích thước payload. Streaming validator phù hợp tải lớn hơn nhưng phức tạp hơn. Chính sách case-insensitive bảo toàn tích hợp cũ nhưng bắt buộc định nghĩa logical key uniqueness.

## 10. What a Senior engineer should notice
Bảo vệ invariant tại trust boundary, không chỉ sau mapping. Phân biệt syntactic JSON validity, DTO deserialization và business-contract validity. Test negative cases và compatibility cùng lúc, quan sát theo partner mà không rò rỉ dữ liệu.
