# Reference Solution — chỉ đọc sau khi tự điều tra

Đây là **một phương án có thể bảo vệ dưới các ràng buộc**, không phải kiến trúc duy nhất đúng. Fixture chỉ chứng minh sự khác biệt dữ liệu; không chứng minh thủ phạm hay một hành vi xâm nhập cụ thể.

## 1. Symptoms

B-4721 có 4 events trong T0 nhưng chỉ còn 3 trong T1; export vẫn 200. Receipt độc lập trước đó ghi 4. Một event approval nằm trong T0 không xuất hiện ở T1; nguyên nhân thực tế chưa được xác minh.

## 2. Evidence

\`Compare-Object\` cho thấy EV-203 chỉ có ở T0. Số lượng trong receipt không khớp T1. Maintenance window là đầu mối cần điều tra, **không phải bằng chứng kết tội DBA**. Timeout export của batch khác và CPU bình thường không giải thích sự khác biệt. Thu thập DB WAL/backup, privileged-access logs, export artifacts, deployment history và provenance; bảo toàn bản gốc, timestamp và quyền truy cập.

## 3. Root cause / trust boundary

**Thiếu một nguồn chứng thực integrity độc lập khỏi miền quyền có thể sửa audit records và export history.** Đây là điểm yếu kiến trúc cần xử lý, không phải kết luận forensic về hành động cụ thể. DB transaction, backup, trigger và application logs đều hữu ích, nhưng nếu cùng một principal có thể sửa dữ liệu và bằng chứng thì không thể độc lập chứng minh lịch sử chưa bị thay đổi.

## 4. Một kiến trúc đề xuất

\`\`\`text
Domain command + audit intent
       │ same DB transaction
       ▼
PostgreSQL business state + transactional outbox
       │ at-least-once relay (dedupe by stable event identity)
       ▼
Per-batch ordered canonical audit journal
       │ sequence + previous digest + current digest
       ├────────────► retention-protected archive (separate write/delete roles)
       ▼
Periodic checkpoint manifest (batch, range, count, head digest, schema version)
       │ sign with protected non-exportable asymmetric key
       ▼
Independently administered checkpoint store / trust root
       │
       ▼
Read-only verifier: replay bytes + ordering + hash links + signature + coverage
       │
       ▼
Evidence report / integrity alarm / proof-gap status
\`\`\`

**Write invariant:** business state change và audit intent cùng một transaction; nếu không ghi được audit intent thì không commit release. Outbox relay có thể chạy lại, do đó event identity + unique constraint phải làm idempotency. Không hứa exactly-once end-to-end.

**Canonicalization:** định nghĩa versioned canonical byte encoding (field order, UTF-8, null, number, timestamp normalization), hash \`H_i = SHA-256(domainSeparator || batchId || sequence || previousDigest || canonicalEventBytes)\`. Không ghép chuỗi mơ hồ. Sequence monotonic trong **mỗi batch**, không giả định global order giữa các cơ sở. Thay đổi schema phải có version và migration/verifier tương ứng.

**Independent checkpoint:** ký manifest có batch, range, count, head digest, schemaVersion, keyId, checkpointTime và previousCheckpointId bằng signing identity không thuộc quyền DB admin. Lưu manifest và public-key provenance trong trust domain khác; key rotation giữ được lịch sử xác minh và xử lý revoke. Digest chain **không đủ** nếu attacker có thể viết lại cả chain và anchor; separation + protected signing/retention mới tạo khả năng phát hiện.

**Verifier:** chạy bằng read-only principal, lấy raw canonical event bytes và checkpoints, kiểm tra contiguous coverage, duplicate, sequence, digest links, signatures, provenance và freshness. Báo \`VERIFIED_THROUGH\`, \`PENDING_ANCHOR\`, \`INTEGRITY_MISMATCH\`, \`SIGNER_COMPROMISE_SUSPECTED\` thay vì một boolean duy nhất.

## 5. Vì sao giải pháp hoạt động

Một lần xóa, thay nội dung hoặc reorder đã xảy ra **sau khi checkpoint được xác thực** sẽ làm mismatch khi replay, trừ khi trust root/khóa ký cũng bị chiếm. Việc chốt checkpoint theo khoảng thời gian làm rõ cửa sổ chưa có proof. Receipt của fixture hiện tại chỉ hỗ trợ điều tra bằng count; **không phải chữ ký mật mã** và không đủ cho A02.

## 6. Verification

Thiết kế fault-injection theo [acceptance tests](../docs/acceptance-tests.md). A01 phải báo thiếu event; A02 phải mismatch digest dù count bằng nhau; A03 mismatch nếu thứ tự thuộc contract. A04 dedupe; A05 backlog chưa anchored; A06 independent verifier; A07 breach SLA/explicit degraded status; A08 verify old signatures; A09 restore reconciliation; A10 incident response và trust revocation; A11 privacy policy; A12 legacy marked \`UNVERIFIED_PRE_ANCHOR\`.

Benchmark riêng: peak 600 events/s, p95 <=300ms cho business write, verifier 1.2m events <=30 phút (~667/s), detection <=15 phút. Đây là **acceptance targets chưa đo**, không phải kết quả production.

## 7. Alternatives / trade-offs

| Option | Ưu điểm | Hạn chế |
|---|---|---|
| DB triggers + WAL + backups | Triển khai nhanh, forensic hữu ích | Cùng trust domain với privileged DB admin; không độc lập |
| Retention-protected object archive | Khó sửa/xóa sau khi ghi, vận hành đơn giản hơn tự xây ledger | Phải kiểm chứng cấu hình retention/roles; không tự chứng minh nguồn và tính đầy đủ |
| Signed external checkpoints + canonical chain + archive | Kiểm chứng độc lập, phát hiện xóa/sửa/reorder sau anchor | Complexity: key custody, canonicalization, replay, gap monitoring |
| Managed immutable audit provider | Giảm gánh vận hành, có thể có attestation | Vendor lock-in, chi phí, compliance và trust model cần thẩm định |

Có thể chọn managed immutable audit service thay vì tự vận hành chain nếu chứng minh được độc lập quản trị, khả năng export/verify, retention và tổng chi phí. Không mặc định tự viết cryptography là tối ưu.

## 8. Wrong / tempting fixes

- **Chỉ bật DB audit trigger:** hữu ích cho điều tra, không độc lập nếu quyền sửa trigger/log thuộc cùng principal.
- **Chỉ tăng backup frequency:** giúp khôi phục, không tự chứng minh bản ghi không bị thay.
- **Chỉ lưu row count:** không phát hiện sửa payload giữ nguyên count.
- **Chỉ SHA-256 từng event trong cùng DB:** kẻ có quyền sửa DB có thể tính lại digest.
- **Đặt signing private key trong appsettings:** phá hỏng separation; secret phải được bảo vệ và quản lý quyền độc lập.
- **Tuyên bố WORM = tamper-proof:** phải kiểm tra retention policy, credential scope, privileged overrides và threat model.
- **Chặn mọi business write khi checkpoint store chậm:** có thể vi phạm latency/SLA; chọn rõ policy, buffer bounded, audit intent durable và trạng thái pending, trừ nghiệp vụ bắt buộc fail-closed.

## 9. Production implications / migration

Rollout: shadow-write audit intent → dual evidence/verification → kiểm tra coverage và chi phí → enforce write invariant → tăng retention và alert. Dữ liệu legacy import với mốc tin cậy mới, không hồi tố xác thực lịch sử. Rollback chỉ rollback reader/presentation; không xóa checkpoint hoặc audit events đã committed. Tính dung lượng: 1.2m × 700 bytes = 0.84 GB/ngày; ×365×5 ≈1.53 TB raw, chưa gồm indexes, copies, metadata, encryption và audit export. Đặt alert cho anchor age, unanchored backlog, verifier mismatch, key custody events, archive retention policy drift. Quyền xóa dữ liệu cá nhân phải được pháp chế xét theo loại dữ liệu, mục đích và nghĩa vụ lưu trữ.

## 10. Requirement change — offline facilities

Mỗi facility có local append-only journal, stable event ID và local sequence/chain, cùng local trusted key/custody nếu threat model cho phép. Đồng bộ qua at-least-once; server giữ **source occurrence order** và **arrival order** riêng, không ép global total order. Checkpoint từ cơ sở offline có provenance và delayed-anchor window; dashboard hiển thị \`PENDING_REMOTE_ATTESTATION\`, không giả vờ verified. Khi WAN phục hồi, reconcile duplicate/gap/sequence và anchor bằng trust root; không tự tin vào local time là thời gian pháp lý. Legacy 18 tháng chỉ được đánh dấu \`IMPORTED_UNVERIFIED_HISTORY\` đến mốc anchor mới. Nếu local key bị chiếm, phải quarantine và điều tra thay vì ký lại lịch sử như chưa có gì xảy ra.

## 11. Senior engineer nên nhận ra

Mục tiêu không phải "dữ liệu không bao giờ bị sửa" mà là **phát hiện thay đổi có phạm vi và giới hạn tin cậy rõ ràng**. Bằng chứng chỉ có giá trị khi biết ai có thể tạo/sửa/khóa/xóa nó, và từ thời điểm nào nó được chốt. Tách reliability, correctness, forensic attribution và cryptographic integrity thành các claim khác nhau; mỗi claim cần acceptance evidence riêng.
