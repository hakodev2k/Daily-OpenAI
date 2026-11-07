# Reference Solution — chỉ xem sau khi đã thử

## 1. Symptoms

Payload không chứa `tag` và payload chứa `"tag": null` đang cho cùng kết quả dù contract yêu cầu hai ý nghĩa khác nhau.

## 2. Evidence

Sau `JsonSerializer.Deserialize<PatchRequest>`, cả hai payload đều tạo `request.Tag == null`. Thông tin property có xuất hiện trên wire hay không đã bị mất.

## 3. Root cause

Một `string?` chỉ mô hình hoá value hoặc `null`; partial update cần tri-state: **missing / null / value**. DTO hiện tại collapse hai trạng thái đầu thành một CLR value.

## 4. Why the fix works

Reference implementation kiểm tra property presence trên `JsonElement` trước, sau đó mới đọc value. Vì vậy missing giữ nguyên state, explicit null xoá state, còn value cập nhật state.

## 5. How to verify

Ba case phải PASS: missing, explicit-null và new-value. Learner chạy `./verify.ps1` trên chính code trong `starter/`.

## 6. Alternative fixes

- Wrapper `Optional<T>` / tri-state type với custom `JsonConverter`.
- JSON Patch nếu API thực sự cần operation-level patch semantics.
- DTO có cờ presence riêng nếu số field ít và contract rất rõ.

## 7. Wrong / misleading fixes

- `if (request.Tag != null)` chỉ tiếp tục mất explicit-null intent.
- Luôn assign `request.Tag` khiến missing field vô tình clear dữ liệu.
- Dùng empty string thay cho null làm thay đổi domain/API contract thay vì giải quyết representation problem.

## 8. Production implications

Partial-update semantics phải được ghi rõ trong OpenAPI/API docs và regression-test ở serialization boundary. Nếu không, client retry hoặc SDK regeneration có thể gây silent data corruption.

## 9. Trade-offs

`JsonDocument` đơn giản cho lab và endpoint nhỏ nhưng dễ trở nên thủ công khi DTO lớn. `Optional<T>` tạo contract type-safe hơn nhưng thêm converter và abstraction. JSON Patch biểu đạt intent mạnh nhưng tăng surface area và validation complexity.

## 10. Senior engineer should notice

Bug không nằm ở `null` đơn thuần mà ở việc domain contract có ba trạng thái trong khi representation chỉ có hai. Khi thiết kế API, luôn kiểm tra wire semantics có survive qua model binding/deserialization hay không.
