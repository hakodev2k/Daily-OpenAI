# Reference Solution — spoiler
Chỉ xem sau khi reproduce, ghi hypotheses và tự sửa. Reference code nằm ở [Program.cs](Program.cs); verify.ps1 chạy learner-editable starter, không chạy solution.

## 1. Symptoms
Direct POST 200 nhưng SPA không gửi được request sau OPTIONS preflight.

## 2. Evidence
Trusted preflight có Origin=https://portal.insurer.test, method POST, headers authorization,content-type,x-client-version. Response thiếu hợp đồng CORS cho browser. HTTP 204 một mình không đủ; direct POST 200 không kiểm tra preflight.

## 3. Root cause
Policy PartnerPortal chỉ cho phép Content-Type và Authorization. SPA thêm X-Client-Version, khiến preflight không đáp ứng danh sách requested headers.

## 4. Why the fix works
Bổ sung X-Client-Version vào WithHeaders, giữ WithOrigins và WithMethods. Trusted preflight hợp lệ; foreign origin và DELETE vẫn không được cấp quyền.

## 5. How to verify
Sửa starter/Program.cs, chạy .\verify.ps1. Probe kiểm tra direct POST, trusted preflight, foreign origin, DELETE. Đừng sửa probe.

## 6. Alternative fixes
AllowAnyHeader với trusted origin và POST: linh hoạt nhưng khó phát hiện contract drift. Bỏ custom header nếu không cần. Reverse proxy same-origin giảm CORS nhưng tăng chi phí routing/vận hành.

## 7. Wrong fixes
AllowAnyOrigin mở phạm vi không cần thiết. Trả OPTIONS 200 mà không có CORS headers vẫn không giúp browser. Tắt browser security chỉ che lỗi. Thêm header vào POST response không sửa preflight.

## 8. Production implications
CORS là browser enforcement, không thay thế authentication/authorization. Lab dùng synthetic token và không triển khai JWT validation.

## 9. Trade-offs
Exact allowlist cần phối hợp SPA/API mỗi khi thay đổi contract; wildcard header linh hoạt hơn nhưng giảm kiểm soát. Giữ negative regression tests trong CI.

## 10. Senior engineering observations
Phân biệt preflight và actual request, HTTP status và browser acceptance; dùng positive/negative tests và đánh giá security boundary trước khi nới policy.
