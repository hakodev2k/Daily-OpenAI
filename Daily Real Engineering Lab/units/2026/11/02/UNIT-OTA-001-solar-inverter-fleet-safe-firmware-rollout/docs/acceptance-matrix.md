# Acceptance Matrix

Với mỗi case, điền before state → event → after state → command được phép/bị chặn → evidence vào workspace.

| Case | Input | Must demonstrate |
|---|---|---|
| A1 | Scheduler đồng thời chọn hai device cùng site | Không quá 1/site INSTALLING/VERIFYING |
| A2 | Snapshot 08:55, threshold 2% | Go/no-go đúng denominator, không tính offline như failed |
| A3 | c-014 attempt=1 SUCCEEDED sau attempt=2 FAILED | Stale ACK không ghi đè state mới |
| A4 | n-044 attempt=2 SUCCEEDED hai lần | Không double-count hoặc duplicate side effects |
| A5 | Campaign PAUSED, offline device reconnect sau 20 phút | Không nhận lệnh mới của campaign đã pause |
| A6 | Single-bank install FAILED | Không báo ROLLED_BACK nếu chưa có recovery evidence |

Kiểm tra thêm: capacity math, signed artifact validation, data residency, emergency stop trong 5 phút, 3 metrics có nguồn dữ liệu, 2 alternatives có trade-offs và 1 trigger để revisit ADR.

Không pass chỉ vì có diagram; mỗi invariant phải có cơ chế thực thi và bằng chứng kiểm tra.
