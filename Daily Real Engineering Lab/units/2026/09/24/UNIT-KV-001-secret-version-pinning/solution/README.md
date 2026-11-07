# Reference Solution — chỉ xem sau khi đã tự điều tra

## Symptoms
Store đã chuyển current version nhưng consumer vẫn đọc giá trị thuộc version cũ.

## Evidence
Store state và consumer output khác nhau ngay sau rotation.

## Root cause
Configuration của consumer chứa cả secret identity và một version cụ thể. Vì vậy thay đổi current version trong store không thay đổi version mà consumer yêu cầu.

## Why the fix works
Giữ configuration ở mức stable secret identity để resolver chọn current version. Trong hệ thống thật cần thêm refresh lifecycle rõ ràng.

Thay đổi tối thiểu cho lab là cấu hình reference chỉ bằng tên secret, không gắn version.

## How to verify
Chạy `./verify.ps1`; verification chạy chính learner-editable `starter/`.

## Alternative fixes
Version pinning vẫn hợp lệ nếu rotation workflow chủ động cập nhật configuration bằng deployment. Cache với TTL hoặc explicit reload cũng có thể phù hợp tùy freshness requirement.

## Wrong fixes
Hard-code giá trị mới, restart mà không đổi reference contract, hoặc polling quá dày đều không giải quyết đúng lifecycle boundary.

## Production implications
Rotation design cần xác định identity, versioning, refresh interval, dependency outage behavior, rollout overlap và rollback.

## Trade-offs
Versionless resolution thuận tiện cho rotation nhưng cần refresh policy. Version pinning tăng reproducibility nhưng làm configuration rollout trở thành một phần bắt buộc của rotation.

## Senior engineer should notice
Secret rotation là lifecycle contract giữa producer, store và consumer; cần biết consumer resolve version khi nào và cache bao lâu.