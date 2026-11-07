# Reference Solution — SPOILER

Chỉ đọc sau khi reproduce, ghi hypothesis và tự thử fix.

## Symptoms / Evidence
Cùng một identity có bucket khác nhau giữa hai replica, khiến beta feature xuất hiện rồi biến mất. Aggregate enable rate vẫn gần target nên che khuất user-level drift.

## Root cause
Starter trộn seed riêng của deployment vào bucket hash. Quyết định cohort vì vậy phụ thuộc vào replica, không chỉ feature + tenant + subject.

## Why fix works
Reference mã hóa ba thành phần bằng UTF-8 length-prefix, dùng SHA-256 deterministic, lấy 64 bit đầu theo big-endian rồi modulo 10.000. Không có node-local seed. Length-prefix giữ identity tuple không nhập nhằng.

## How to verify
Sao lưu starter/FlagEvaluator.cs, chép solution/FlagEvaluator.cs đè vào starter, chạy ./verify.ps1. Test phải pass. ./reproduce.ps1 sẽ trả nonzero vì lỗi cũ đã biến mất. Không sửa Program.cs để làm xanh test.

## Alternatives
- Stable HMAC-SHA256 với shared policy salt nếu cần giảm khả năng suy đoán cohort; có chi phí secret rotation.
- Persistent assignment table nếu cohort tuyệt đối không đổi dù thay thuật toán hoặc identity key; thêm storage và consistency.
- Fixed/versioned non-cryptographic hash nếu không cần chống thao túng; phải kiểm tra compatibility cross-language.

## Wrong / tempting fixes
- Sticky load-balancer session che lỗi nhưng fail khi restart/failover.
- Đặt cùng seed trên tất cả replica giảm triệu chứng nhưng vẫn có nguy cơ rebucket khi thay config.
- Cache cục bộ không đảm bảo nhất quán xuyên node.
- Chỉ kiểm tra aggregate rollout rate không đo user-level stability.

## Production implications / Trade-offs
Không âm thầm áp dụng hash mới lên cohort đang hoạt động: nó sẽ thay assignment cũ. Dùng policyVersion, dual-evaluation telemetry, migration/rollback plan. Identity phải lấy từ authenticated claims, không từ untrusted header. Hash không phải authorization hoặc anonymization.

## Senior engineer should notice
Feature rollout SLO phải bao gồm cohort stability, monotonicity, identity boundary, policy-version semantics và kiểm chứng rollout/rollback. Chỉ log hashed correlation keys nếu cần telemetry; không log raw sensitive IDs.
