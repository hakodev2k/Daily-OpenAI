# Decision worksheet — điền trước khi xem reference

## 1. Observation / Hypothesis

| Evidence | Điều quan sát được | Giả thuyết | Cách bác bỏ |
|---|---|---|---|
| T0 vs T1 | | | |
| Receipt vs T1 | | | |
| Timeline | | | |

## 2. Trust boundaries

Vẽ ít nhất bốn vai trò: domain API, DB/export admin, independent verifier, key/checkpoint custodian. Ghi quyền đọc, sửa, xóa, ký, xác minh của từng vai trò. Nêu điều gì xảy ra nếu một vai trò bị chiếm.

## 3. ADR

- **Decision drivers:** integrity detection, business latency, 5-year retention, operator burden, cost.
- **Options:** A — DB trigger + backup; B — append-only/retention storage; C — independently verifiable checkpoint design; hoặc phương án khác do bạn bảo vệ.
- **Chosen option:** …
- **Why alternatives lose under these constraints:** …
- **Residual risk and non-goals:** …
- **Rollback condition:** …

## 4. Design details

- Sự kiện nào được coi là committed? Cách liên kết business transaction và audit?
- Xác định canonical bytes, identity, ordering scope, digest, checkpoint, signing key và trust anchor.
- Xử lý retry, duplicate, gap, out-of-order, signer outage, partition, reprocessing.
- Verification chạy độc lập bằng những input nào, xuất bằng chứng gì?
- Migration dữ liệu cũ: có thể bảo đảm những gì và không thể hồi tố điều gì?

## 5. Acceptance evidence

Điền từng case trong [acceptance-tests.md](acceptance-tests.md): detector nào phát hiện, thời hạn, bằng chứng, expected alert. Đừng chỉ ghi "test pass".
