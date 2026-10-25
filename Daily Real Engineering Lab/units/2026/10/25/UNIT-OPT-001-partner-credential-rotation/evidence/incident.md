# Incident evidence (dữ liệu mô phỏng)

Các dòng sau là **kết quả dự kiến từ code starter khi chạy**, chưa phải log đã thực thi trong phiên tạo lab.

~~~text
11:20:00 initial=Accepted config=v1
11:22:10 CONFIG_CHANGED Partner:CredentialVersion=v2
11:22:11 rotation2 provider=v2 partnerControl=Accepted gateway=Unauthorized
11:25:03 CONFIG_CHANGED Partner:CredentialVersion=v3
11:25:04 rotation3 provider=v3 partnerControl=Accepted gateway=Unauthorized
11:25:05 oldVersionControl=Unauthorized
11:25:06 NETWORK p95=34ms (trong giới hạn 100ms)
~~~

**Giả thuyết cạnh tranh:** đối tác downtime; lỗi network; thay đổi config chưa phát tán; instance gateway giữ state cũ; bản ghi xét nghiệm không hợp lệ. Không suy diễn root cause chỉ từ một dòng Unauthorized.

**Bằng chứng cần tự thu thập:** chạy `run.ps1`, so sánh provider version với version đi vào `PartnerEmulator`; kiểm tra kết quả control call; kiểm tra thời điểm khởi tạo service; ghi lại trường hợp sau lần rotation thứ hai. Không ghi log credential thật trong hệ thống production.
