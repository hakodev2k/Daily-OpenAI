# Reference Solution — chỉ đọc sau khi tự làm

## Symptoms / Evidence
Issuer ký K2 từ 09:00 nhưng các cache đối tác chưa biết K2. Token K2 hợp lệ lúc 09:01 bị báo `unknown_kid` tại A/B/C cho tới lần refresh tương ứng. Đây không phải lỗi hết hạn token. Đo `kid` thất bại, thời điểm cache refresh, signature verification result, không ghi raw JWT vào log.

## Root cause
Lifecycle khóa ký và lifecycle phân phối public key không đồng bộ. Switch signing key trước khi tất cả verifier biết public key mới tạo khoảng không xác thực được. Loại bỏ K1 quá sớm lại khiến token K1 chưa hết hạn bị từ chối.

## Một thiết kế có thể bảo vệ
1. Generate K2 trong KMS/HSM, công bố **chỉ public key** cùng K1 qua JWKS. `kid` không tái sử dụng. Bắt đầu prepublish tối thiểu 120 phút trước switch dự kiến, thêm margin cho clock drift, network delay và missed refresh. Nếu partner không xác nhận cập nhật thì hoãn switch hoặc thỏa thuận failover.
2. Kiểm tra telemetry từ A/B/C cho thấy K2 có trong keyset, endpoint JWKS có cache headers hợp lệ và kiểm tra signature trên canary token K2 ở môi trường kiểm soát.
3. Activate K2 chỉ khi các gate đạt; ngừng phát hành K1. Giữ K1 trong JWKS và verifier trust set ít nhất 45+2 phút sau **token K1 cuối cùng** cộng margin. Không coi TTL 45 phút là đủ để phân phối K2 trước switch.
4. Retire K1 sau khi không còn token K1 hợp lệ, xác nhận propagation và ghi audit. Không bao giờ public private key.
5. Rollback planned rotation: nếu K1 chưa compromised, có thể tạm ký lại K1 trong lúc K2 vẫn được công bố, sau khi kiểm tra trạng thái verifier. Không rút K2 ngay vì token K2 đã phát hành vẫn có thể còn hạn.
6. Compromise K1 tại 10:20: ngừng tin K1 càng sớm càng khả thi, điều phối revoke/denylist, notify partners và chấp nhận ảnh hưởng token K1 hợp lệ. Rollback về K1 là không an toàn. Điều tra scope, rotate credentials, kiểm tra key provenance và log access.

## Verification
Ma trận planned rotation: với token ký K1 ngay trước switch, verifier còn K1 cho đến expiry+skew; với token K2 ngay sau switch, cả A/B/C đã nhận K2 trước switch. Kiểm thử missing kid, signature tamper, `alg` confusion, invalid issuer/audience, expired/not-before, stale JWKS, offline partner và emergency revoke. SLO chỉ có thể bảo đảm nếu refresh contract có upper bound; cache 120 phút không bảo đảm đối tác thực sự refresh đúng giờ.

## Alternatives và trade-offs
- **Overlap JWKS**: đơn giản, ít thay đổi, phụ thuộc refresh discipline.
- **Out-of-band partner acknowledgment**: tăng confidence, tăng chi phí tích hợp và coordination.
- **Introspection/online verification**: revoke nhanh hơn nhưng thêm latency, dependency và outage mode; cần hợp đồng mới.

## Wrong fixes
- Tăng token TTL: làm kéo dài thời gian phải giữ K1 và rủi ro compromise.
- Chỉ tăng retry API: không khiến offline verifier có K2.
- Xóa K1 ngay sau switch: phá token K1 còn hạn.
- Bỏ signature verification khi gặp unknown kid: tạo security bypass.
- Rollback sang K1 sau compromise: tiếp tục sử dụng trust anchor đã mất an toàn.

## Senior engineer nên nhận ra
Rotation là protocol liên tổ chức, không chỉ thay secret trong issuer. Phải định nghĩa key lifecycle, propagation guarantees, fail-safe policy, observability, ownership và incident mode riêng; không thể đồng thời hứa zero downtime và immediate revocation cho mọi token khi khóa đã bị lộ.
