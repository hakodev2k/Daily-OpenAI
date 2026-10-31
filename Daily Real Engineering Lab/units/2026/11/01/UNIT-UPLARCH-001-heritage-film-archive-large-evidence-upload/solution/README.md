# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms
Upload 2 GiB đồng thời gây lỗi hoặc vượt SLO; upload chưa qua kiểm duyệt có nguy cơ lộ; retry tệp lớn tốn thời gian.

## 2. Evidence
16 × 2048 = 32768 MiB. Ngân sách memory 1536 MiB/instance; proxy outbound 80 MiB/s/instance; SLO 300 giây. Bản starter dự kiến 32896 MiB và 410 giây trên một instance.

## 3. Root cause / decision drivers
Thiết kế hiện tại giữ toàn bộ payload ở API process và không có lifecycle nhất quán cho upload session → integrity → scan → publish. Tăng timeout không khắc phục memory; upload thành công không đồng nghĩa file an toàn để phát hành.

## 4. Một kiến trúc có thể bảo vệ được
Chọn **direct staged block upload**. API xác thực người gửi và tenant, cấp quyền upload ngắn hạn chỉ cho object/session, client gửi block theo manifest và resume theo block ID. Finalize idempotent kiểm tra ownership, kích thước và checksum; lưu trạng thái PENDING_SCAN. Worker quét malware, chỉ sau PASS mới đổi trạng thái PUBLISHED và cấp quyền download. Reconcile session hết hạn, dọn block bỏ dở; giữ audit bất biến và chính sách retention.

## 5. Vì sao hoạt động
Byte dữ liệu không đi qua memory hoặc outbound của ASP.NET Core, giảm áp lực tài nguyên. Upload có thể resume; quyền ghi tạm không phải quyền đọc; gate kiểm duyệt ngăn publish sớm. Mô hình cho 137 giây và 128 MiB memory app/instance.

## 6. Verify
Chạy verify.ps1 trên workspace sau khi tự sửa. Có thể đối chiếu solution/architecture.json bằng cách sao chép thủ công **sau** khi đã thử. Tạo test thực cho resume, checksum mismatch, token expiry, cross-tenant access, finalize duplicate, scanner failure và abandoned upload cleanup.

## 7. Alternative fixes
**streamed-proxy, 2 instances** cũng đạt capacity giả định (205 giây, 192 MiB/instance, cost 300), phù hợp nếu network client không thể truy cập Blob trực tiếp. Cần bounded streaming, backpressure, cancellation và retry/resume riêng. **buffered-proxy** chỉ hợp lý với giới hạn file nhỏ và concurrency thấp, không phù hợp ràng buộc này.

## 8. Wrong / tempting fixes
- Chỉ tăng request timeout: không giải quyết memory hoặc scan gate.
- Chỉ tăng số instance: không khắc phục việc một tệp 2 GiB vượt memory/instance.
- Chỉ dùng SAS dài hạn: tăng nguy cơ lộ token và ghi vượt phạm vi.
- Publish ngay sau khi upload xong: bỏ qua malware/integrity.
- Retry toàn bộ tệp khi mất kết nối: lãng phí băng thông và làm nặng incident.

## 9. Production implications
Cần đo throughput thực, giới hạn request, chi phí transaction/storage/egress, audit 7 năm, retention lock nếu compliance yêu cầu, CORS, token scope/TTL, virus scan capacity, private endpoint và quy trình revoke. Không suy luận rằng chi phí cloud bằng compute-only model.

## 10. Trade-offs
Direct upload giảm tải API nhưng phức tạp hóa client, token issuance và state machine; streamed proxy đơn giản hơn về network boundary nhưng cần thêm capacity và HA. Cả hai có thể hợp lệ tùy hạ tầng.

## 11. Senior engineer should notice
Capacity gate và security/lifecycle gate độc lập; trạng thái PENDING_SCAN không được hiển thị như PUBLISHED; finalize phải idempotent dưới retry; quyền ghi phải ràng buộc tenant/object; cần có cleanup và observability cho session mồ côi. Quyết định cần ghi lại giả định có thể kiểm chứng, không chỉ chọn công nghệ.
