# Reference Solution — UNIT-CMPID-001

## Root cause
Starter khởi tạo `Dictionary<string,string>` bằng `StringComparer.CurrentCultureIgnoreCase`. Quy tắc casing của `tr-TR` phân biệt ASCII `I` và `i`, trong khi `I` và dotless `ı` là một cặp chữ theo tiếng Thổ Nhĩ Kỳ. Registry dùng comparer này cho cả lookup và duplicate detection, khiến kết quả phụ thuộc locale khi kiosk được khởi tạo.

## Fix
Thay comparer ở **một điểm khởi tạo** bằng `StringComparer.OrdinalIgnoreCase`. Không sửa harness. Có thể đối chiếu [reference code](AccessCodeRegistry.cs).

```powershell
Copy-Item ./solution/AccessCodeRegistry.cs ./starter/AccessCodeRegistry.cs -Force
./verify.ps1
```

Sau fix, cả `en-US` và `tr-TR` tìm được `inv-42` cho mã gốc `INV-42`, từ chối dotless `ıNV-42`, và từ chối đăng ký trùng `inv-42`.

## Wrong fixes
1. **Ép `ToLower()` trước lookup**: overload mặc định vẫn phụ thuộc culture; còn tạo thêm biểu diễn key và rủi ro bất nhất giữa writer/reader.
2. **Đổi locale toàn ứng dụng thành `en-US`**: che giấu coupling, ảnh hưởng format ngày, số và các module khác.
3. **Dùng `StringComparer.Ordinal`**: an toàn với culture nhưng phá hợp đồng cho phép ASCII hoa/thường tương đương.
4. **Sửa chỉ nhánh lookup, giữ `TryAdd` cũ**: duplicate detection vẫn sai.

## Trade-offs và production rollout
- OrdinalIgnoreCase áp dụng cho identifier theo hợp đồng đã xác nhận; không thay thế locale-aware sorting cho tên người hoặc văn bản hiển thị.
- Nếu identifier là secret token, lựa chọn mặc định thường là so sánh chính xác theo protocol, không tự động bỏ qua case.
- Khi thay comparer trên kho dữ liệu có sẵn, kiểm tra collision/migration trước; hai key trước đây khác nhau có thể trở thành một.
- Thêm contract tests cho culture và ký tự lookalike vào CI; đo tỷ lệ `unknown_code` theo kiosk sau rollout, tránh log toàn bộ mã vé nhạy cảm.

## Verify
`./verify.ps1` chạy **starter đã sửa**. `./reproduce.ps1` chỉ dành cho triệu chứng starter nguyên bản, không phải acceptance test sau fix.
