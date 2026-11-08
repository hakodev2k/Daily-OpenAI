# Verification — decision tabletop

Tự đánh dấu PASS/FAIL và ghi bằng chứng vào workspace. Đây là kiểm tra phương án người học, không phải kiểm tra reference solution.

| ID | Acceptance gate | Bằng chứng phải cung cấp |
|---|---|---|
| V1 | Không query index đang rebuild | read-target timeline + atomic switch |
| V2 | Không bỏ mất seq 501–505 | checkpoint snapshot + replay trace |
| V3 | Duplicate/out-of-order không làm lùi revision | per-record revision guard |
| V4 | Gỡ công khai R-0077 không lộ sau cutover | deletion/tombstone visibility policy |
| V5 | Không chỉ dựa vào count/health | high-water mark + sampling/reconciliation |
| V6 | Rollback trong 5 phút nhưng không hồi sinh dữ liệu cũ | điều kiện rollback, dual-write/replay/fence |
| V7 | Có đo p95, errors, lag, completeness | monitoring và abort threshold |
| V8 | Phương án phù hợp 3 engineers và dual-run <=48h | trade-off + vận hành |
| V9 | Có phương án xử lý stream bị gap | stop cutover + recover/rebuild checkpoint |
| V10 | Không ngừng DB writes quá 10 giây | deployment sequence |

**Bài test đối kháng:** giả sử seq 505 đến trước seq 502 và seq 504 bị retry hai lần. Hãy giải thích trạng thái cuối cùng của R-0420 và R-0077. Không được kết luận chỉ từ tổng document count.

**Kết quả:** PASS chỉ khi cả V1–V10 có bằng chứng cụ thể; thiếu một gate thì chưa sẵn sàng cutover.
