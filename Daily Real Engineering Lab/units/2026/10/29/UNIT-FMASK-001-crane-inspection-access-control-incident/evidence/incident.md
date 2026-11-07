# Evidence — bản ghi mô phỏng tại thời điểm rollout

Các dòng dưới đây là dữ liệu giả lập dùng trong lab, không phải production log.

| Time (UTC+7) | User permissions | Action | Policy snapshot | Decision |
|---|---|---|---|---|
| 09:01:02 | ViewInspection | ViewInspection | config-v18 | ALLOW |
| 09:01:09 | ViewInspection | ApproveInspection | config-v18 | DENY |
| 09:02:16 | ViewInspection | ExportCertificate | config-v18 | ALLOW |
| 09:02:20 | ViewInspection | ExportCertificate | config-v18 | ALLOW |

Rollout note: bản config-v18 thêm nút xuất biên bản vào giao diện. Tài khoản kiểm thử vẫn chỉ có quyền ViewInspection. Không có thay đổi danh tính, token hoặc quyền người dùng.

Các hypothesis nên cân nhắc: policy resolution, user permission mapping, frontend/backend divergence. Hãy xác nhận bằng kết quả chạy thật của starter.