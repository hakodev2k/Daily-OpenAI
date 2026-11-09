# Acceptance matrix (điền evidence và verdict)
| Case | Injection | Invariant / oracle | Evidence cần thu |
|---|---|---|---|
| A1 | Broker gửi trùng ALERT-41 | Không tăng số lần actuate vì duplicate | command ID + actuation audit |
| A2 | ALL_CLEAR-42 trước ALERT-41 | ALERT-41 không được kích hoạt lại | version fence |
| A3 | Gateway mất mạng 30 phút | Không gán trạng thái confirmed giả | heartbeat + dashboard |
| A4 | Relay lỗi | Accepted không được coi là confirmed | relay feedback |
| A5 | ACK bị mất sau khi relay bật | Retry không bật lặp; trạng thái unknown đến khi reconcile | idempotency + readback |
| A6 | Clock lệch ±90 giây | Quy tắc hết hạn không phụ thuộc mù quáng vào đồng hồ không tin cậy | validity policy |
| A7 | ALL_CLEAR đến sau khi đã bật | Chuyển trạng thái theo thứ tự, có ACK thực thi | command ordering |
| A8 | Local sensor kích hoạt offline | Ưu tiên safety, có audit + reconcile và phân quyền | local policy |
| A9 | Dashboard không có heartbeat >10s | Hiển thị unknown + escalation, không active | heartbeat alert |

Một case chỉ PASS khi có invariant, cơ chế, test oracle và bằng chứng cụ thể; không coi script kiểm tra tài liệu là chứng minh vận hành.