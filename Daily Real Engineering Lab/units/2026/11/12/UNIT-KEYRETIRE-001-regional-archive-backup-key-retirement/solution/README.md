# Reference Solution — UNIT-KEYRETIRE-001

## Root cause
Chính sách retirement chỉ giữ phiên bản khóa mới nhất. Backup `archive-jan` và `archive-feb` chưa hết hạn retention nhưng khóa giải mã tương ứng đã bị loại. Checksum không xác minh được khả năng giải mã.

## Fix
Tính tập phiên bản khóa cần giữ từ mọi backup chưa hết hạn, chỉ retire key khi không còn backup phụ thuộc. Dùng inventory theo key ID/version, bảo vệ quyền decrypt riêng với quyền rotate/delete; không xuất key material vào logs.

## Verification
`./verify.ps1` chạy trực tiếp starter do người học chỉnh sửa. Có thể so sánh với `dotnet run --project solution/ArchiveReplay.csproj -- --verify`. Kỳ vọng `unrestorable=0`.

## Architecture decision
Không xóa phiên bản khóa trước khi backup cuối cùng phụ thuộc hết hạn và đã có bằng chứng restore drill. Duy trì deletion hold, dual control, audit log, monitoring restore success và kiểm tra inventory theo lịch. Nếu chính sách bảo mật yêu cầu cryptographic erasure trước retention, phải giải quyết mâu thuẫn chính sách với chủ sở hữu dữ liệu trước, không lặng lẽ bỏ khả năng restore.

## Wrong fixes
- Chỉ kiểm tra checksum: không xác nhận giải mã.
- Chỉ giữ key mới nhất: làm mất restore cho bản cũ.
- Re-encrypt backup không kiểm tra và không có rollback: có thể gây mất dữ liệu hàng loạt.
- Giữ tất cả key vô thời hạn: tăng blast radius và không đáp ứng data deletion policy.

## Trade-offs
Giữ phiên bản khóa tăng chi phí vận hành và phạm vi rủi ro; rewrap key có thể giảm số phiên bản nhưng cần migration có kiểm chứng. Restore drill phải bao gồm backup cũ nhất còn trong retention, kiểm tra RTO/RPO và phân quyền khôi phục.
