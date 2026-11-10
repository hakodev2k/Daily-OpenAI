# Reference — Spoiler

## Root cause and limits
Digest khác không tự chứng minh tấn công: ảnh offline có thể đến sau E-01. Thiếu immutable provenance và export revision làm hệ thống không thể chứng minh khác biệt là hợp lệ hay trái phép.

## Design
1. Client lưu original bytes bền vững, upload chunk có idempotency key (deviceId, localCaptureId). Server kiểm full SHA-256 trước khi chuyển RECEIVED -> VERIFIED.
2. Lưu immutable content-addressed blob; metadata claimedCaptureTime tách serverReceivedAt và monotonic ingestSequence.
3. Custody log append-only gồm actor, action, artifact hash, previous event, server sequence, reason; bảo vệ log bằng checkpoint/signature và độc lập giám sát quyền admin.
4. Annotation là revision; conflict offline giữ cả hai revision và yêu cầu review, không last-write-wins. Redaction tạo derivative artifact, không sửa original.
5. Mỗi export là snapshot có exportId, revision, previousExportId, ordered hashes, custody checkpoint, keyId, timestamp và chữ ký canonical manifest. E-01 giữ nguyên; E-02 liên kết E-01 và giải thích ảnh đến muộn.
6. Key rotation/revocation có chính sách xác minh lịch sử, xử lý compromise và re-attestation; không giả định chữ ký tự chứng minh ai chụp ảnh.

## Verification oracle
V1 dedupe; V2 retain revisions; V3 hash/signature fail; V4 independent server order; V5 both exports verify; V6 incident response; V7 no partial VERIFIED; V8 access-safe redacted export.

## Alternatives
DB audit log đơn lẻ dễ bị privileged tamper; object versioning không đủ chứng thực tập export; blockchain thêm chi phí mà không giải quyết source authenticity. Hardware attestation có giá trị trên managed devices nhưng không phổ quát.

## Rollout
Dual-write và reconcile trước cutover; theo dõi mismatches, verification failures, orphan uploads, key health. Legacy evidence đánh dấu trust level, không giả chữ ký lịch sử.
