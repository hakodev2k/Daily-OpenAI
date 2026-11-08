# Hợp đồng và ràng buộc

- Issuer: một dịch vụ ký RS256, JWKS public chỉ chứa public keys; mỗi key có `kid` duy nhất.
- Audience và issuer phải được kiểm tra độc lập; không dùng `alg=none` hay chấp nhận algorithm từ input.
- Token TTL tối đa 45 phút; clock skew chấp nhận tối đa 2 phút.
- Đối tác A refresh JWKS theo chu kỳ 15 phút, B 60 phút, C 120 phút; không có push/invalidation tức thời.
- Token ký ở thời điểm T có thể được sử dụng đến T+45 phút; không giả định client tiêu thụ ngay.
- SLO: planned rotation không làm rớt token hợp lệ; sự cố lộ private key phải ưu tiên containment và có thể chấp nhận gián đoạn có kiểm soát.
- Không được đưa private key vào JWKS hoặc logs.
- Nhóm vận hành: 2 backend, 1 SRE; không triển khai thay đổi verifier đồng thời trong 3 tổ chức.
- Đầu bài: đề xuất trình tự publish -> observe -> activate -> observe -> retire, đo thời gian chờ an toàn, xác định phương án rollback.
- Requirement change: tại 10:20 có bằng chứng private key cũ bị lộ; mô tả vì sao planned rotation policy không đủ.
