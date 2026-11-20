# Reference solution — SPOILER

## Root cause
CI #841 push lại mutable tag 2026.11.20. A chạy digest aaa111 và B chạy bbb222, dù UI label giống nhau. Tag không phải immutable release identity. Chưa thể khẳng định thay đổi business logic nào gây sai số trước khi diff source/config và golden tests.

## Investigation
Thu thập Pod imageID, deployed manifest, registry digest, Git commit, attestation, config/feature flags, raw payload. CPU và DNS là tín hiệu nhiễu chưa đủ giải thích.

## Containment
Dừng promotion; canary artifact digest đã được phê duyệt dựa trên golden payload (không mặc định A đúng). Deploy bằng full digest, kiểm tra runtime imageID, so sánh golden response và error/latency; rollback hoặc fail-safe khi không đạt. Đối soát dữ liệu đã xử lý, tránh replay gây duplicate.

## Prevention
Build một lần từ pinned commit; ký artifact, lưu SBOM/provenance; promote cùng digest giữa môi trường. Enforce tag immutability/permissions, retention cho rollback. Post-deploy xác minh imageID, release manifest và synthetic tests.

## Trade-offs
Immutable tag giúp dễ đọc nhưng vẫn cần registry policy; digest pinning xác định binary nhưng cần metadata tốt; rebuild theo môi trường giảm auditability. Nếu digest cũ đã bị GC, không hứa rollback chính xác; chọn artifact còn tồn tại đã xác minh hoặc build lại có kiểm chứng với RTO risk. Nếu imageID giống nhau nhưng behavior khác, tiếp tục điều tra config và dữ liệu.
