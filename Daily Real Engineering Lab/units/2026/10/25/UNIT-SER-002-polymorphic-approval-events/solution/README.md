# Reference Solution — chỉ mở sau khi tự làm

## Symptoms
JSON audit lưu đúng hai event ID nhưng thiếu người phê duyệt, policy version và lý do từ chối.

## Evidence
Reproduce cho thấy dữ liệu ở object đầu vào đầy đủ nhưng persisted JSON chỉ có property của base event. Verify ban đầu thất bại ở wire contract và replay.

## Root cause
System.Text.Json đang serialize một IReadOnlyList<ApprovalEvent> theo declared base contract. Không có polymorphism metadata, các property chỉ tồn tại ở derived event không nằm trong payload. Abstract base event không đủ để deserialize lại subtype.

## Why the fix works
Reference ApprovalEvents.cs đăng ký subtype bằng JsonPolymorphic và JsonDerivedType, có discriminator $eventType ổn định. Serializer giữ derived fields; deserializer có thể chọn đúng subtype và mặc định từ chối discriminator lạ.

## How to verify
Chạy ./verify.ps1 sau khi sửa starter. Để đối chiếu reference độc lập, chạy:
    dotnet run --project solution/Lab.Reference.csproj -- verify
Reference dùng chung test harness trong starter/Program.cs nhưng class event riêng của solution.

## Alternative fixes
Versioned DTO envelope với type/schemaVersion/payload và registry; custom JsonConverter với whitelist; các lựa chọn này hợp lý nếu schema được triển khai độc lập bởi nhiều service.

## Wrong fixes
Dùng List<object> có thể ghi runtime properties nhưng thường replay ra JsonElement mà không có subtype contract; ép kiểu chỉ ở chiều ghi không giải quyết chiều đọc; đưa tất cả trường của mọi subtype lên base tạo trạng thái thiếu ràng buộc.

## Production implications
Quản trị discriminator như public contract; thêm versioning, validation, telemetry cho unknown type, quarantine khi gặp event mới, kiểm soát PII và chính sách giữ audit. Dữ liệu lịch sử đã bị mất trường không tự phục hồi chỉ nhờ deploy code.

## Trade-offs
Attribute registry đơn giản và ít code khi số subtype nhỏ; DTO envelope rõ ràng hơn với schema evolution dài hạn. Không tự động coi polymorphism là giải pháp migration.

## Senior engineer should notice
Đo và so sánh wire representation trước khi đổ lỗi cho persistence. Regression tests phải kiểm tra shape, round-trip, unknown type, backward compatibility và dữ liệu cũ.
