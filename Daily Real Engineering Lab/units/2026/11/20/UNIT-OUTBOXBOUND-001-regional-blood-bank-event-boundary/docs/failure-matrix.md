# Failure injection / verification matrix
Với mỗi ca, ghi `PASS/FAIL`, durable record còn lại, thao tác retry, invariant được giữ, metric và thời điểm cảnh báo.

| Case | Injection | Bất biến cần giữ |
|---|---|---|
| F1 | API crash trước SQL COMMIT | Không có Released, không phát sự kiện |
| F2 | API crash ngay sau SQL COMMIT | Sự kiện vẫn có thể phát lại sau restart |
| F3 | Broker nhận message nhưng ACK mất | Publish retry không gây tác động nghiệp vụ trùng |
| F4 | Dispatcher crash sau publish trước đánh dấu hoàn tất | Redelivery/publish lại an toàn |
| F5 | Consumer crash sau tạo shipment trước ACK | Không có shipment thứ hai |
| F6 | Version 12 tới trước 11 | Không lùi trạng thái hoặc xử lý transition sai |
| F7 | Broker outage 24h, sau đó phục hồi | Backlog bền vững, replay có giới hạn và quan sát được |
| F8 | Poison message luôn fail | Không chặn vô hạn các sự kiện hợp lệ; có quarantine và runbook |
| F9 | Hai dispatcher tranh cùng record | Không bỏ sót; tránh publish đồng thời không kiểm soát |
| F10 | Deployment thêm field mới | Consumer cũ vẫn đọc được hoặc có migration plan rõ ràng |

Tiêu chí PASS: giải thích chính xác boundary của atomicity; không tuyên bố distributed exactly-once; mô tả key uniqueness và transaction consumer; có retry/backoff, metrics, alert, recovery, retention; chỉ ra ít nhất một trade-off có chi phí.
