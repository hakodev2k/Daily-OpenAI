# Decision record — điền trước khi xem lời giải

## Triệu chứng và evidence
- 
## Hypotheses (ít nhất 3)
1. 
2. 
3. 
## Compatibility matrix
| Schema state | V1 read | V1 write | V2 read | V2 write | Rollback V1 |
|---|---|---|---|---|---|
| S0: original | | | | | |
| S1: expanded | | | | | |
| S2: backfilled + dual compatibility | | | | | |
| S3: contracted | | | | | |

## Hai phương án và trade-offs
- A:
- B:
- Quyết định:

## Timeline rollout và rollback
- Từng bước:
- Khi nào rollback:
- Xử lý worker cũ và concurrent writes:

## Gates định lượng
- error rate:
- backfill lag:
- data divergence:
- worker/version inventory:
- rollback drill:

## Requirement change
V2 lỗi ở 20% traffic; cần rollback trong 10 phút. Mô phỏng chi tiết.

## Verification
Chứng minh: V1 ghi khi V2 đang chạy; V2 ghi khi V1 đang chạy; retry; backfill race; rollback; audit.
