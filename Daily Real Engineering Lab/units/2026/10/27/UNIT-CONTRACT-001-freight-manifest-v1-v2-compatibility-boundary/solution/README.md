# Reference Solution — SPOILER

**Chỉ xem sau khi reproduce và tự sửa.** [Reference code](ManifestIngress.cs) là một phương án hợp lý, không phải kiến trúc duy nhất.

## 1. Symptoms
Valid v1 bị reject; một số invalid v2/unknown version được accept.

## 2. Evidence
Các contract cases tái hiện lỗi mà không cần network hay database. CPU/DB ổn định, nên capacity không phải giả thuyết ưu tiên.

## 3. Root cause
Starter deserialize mọi payload qua DTO v2. V1 không có grossWeightKg nên mặc định 0; v2 thiếu hazardClass lại được gán `NONE`; version không được kiểm tra.

## 4. Why the fix works
Chọn contract tại ingress, validate từng version rồi normalize. V1 dùng `NOT_APPLICABLE` (chưa khai báo) chứ không được tự xác nhận `NONE`. Unknown và mixed-field payload bị reject.

## 5. How to verify
Sau khi tự thử, có thể `Copy-Item solution/ManifestIngress.cs starter/ManifestIngress.cs -Force` rồi `./verify.ps1`. Cả 8 cases phải PASS. Mở rộng tests cho duplicate JSON properties, depth/size limits, whitespace IDs, invalid hazard, schemaVersion=1.

## 6. Alternative fixes
Versioned routes /v1 và /v2 rõ ràng nhưng tăng routing surface; media-type versioning phù hợp nếu partner kiểm soát header; envelope version field ít thay đổi endpoint nhưng đòi hỏi schema governance.

## 7. Wrong / tempting fixes
String replace tên field làm sai JSON; default missing hazard thành `NONE` gây sai dữ liệu; cho unknown version qua v2 parser tạo silent corruption; xóa negative tests che mất lỗi.

## 8. Production implications
Giới hạn request body, chuẩn hóa error responses, metrics theo partner/version, không log PII, canary trên sanitized replay. Downstream không được coi `NOT_APPLICABLE` là non-hazardous.

## 9. Trade-offs
Dual adapters thêm test surface nhưng giữ compatibility và cô lập version branching. Hai pipeline end-to-end dễ diverge; v2-only đơn giản trước mắt nhưng làm hỏng 35% traffic.

## 10. What a Senior engineer should notice
Behavioral compatibility quan trọng hơn deserialize thành công. Cần canonical invariants, telemetry, rollback, partner communication và sunset có bằng chứng.

## Example ADR
Chọn dual adapters, shadow validation 7 ngày, canary theo partner 5%, giữ v1 ít nhất 60 ngày, rollback nếu valid-v1 rejection tăng hoặc canonical data divergence. Partner C gửi v3 sớm phải bị reject rõ ràng, không suy đoán.
