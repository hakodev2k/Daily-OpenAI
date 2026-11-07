# Reference Solution — chỉ xem sau khi reproduce và tự thử fix

## 1. Symptoms
360 event trong fixture tạo ra 360 active series, vượt budget 36. Production tăng sensor/request khiến số chuỗi tăng dù API CPU và error rate bình thường.

## 2. Evidence
Metric labels hiện gồm `region`, `depot`, `sensor_id`, `outcome`, `request_id`. Request ID gần như duy nhất nên metric cardinality xấp xỉ số event; RTT tăng nhẹ không giải thích được hiện tượng.

## 3. Root cause
Metric dùng dimension mang identity cấp request/sensor cho một chỉ số aggregate. Cardinality của tập tổ hợp labels tăng không bị chặn bởi số nhóm vận hành.

## 4. Why the fix works
Dùng policy mẫu `solution/metric-policy.json`: giữ `region`, `depot`, `outcome`. Với fixture, 6 depot × 3 outcome = **18 active series**; các nhóm vận hành vẫn đầy đủ. Không đổi số event đầu vào.

## 5. How to verify
Copy nội dung policy tham khảo vào `starter/metric-policy.json` **chỉ sau khi đã thử**:
```powershell
pwsh -NoProfile -File ./run.ps1
pwsh -NoProfile -File ./verify.ps1
```
`verify.ps1` kiểm tra trực tiếp **starter policy người học đã sửa**, không kiểm tra riêng reference. Kỳ vọng PASS 18/36.

## 6. Alternative fixes
- Aggregate theo region/outcome: cardinality thấp hơn nhưng không đáp ứng drilldown theo depot.
- Aggregate theo region/depot/outcome (đề xuất): đáp ứng dashboard và budget fixture.
- Dùng metric riêng theo sensor cho nhóm cảm biến critical có quota, retention và alert rõ ràng: có thể hợp lệ nếu ngân sách/nhu cầu đã được chứng minh; không mặc định bật toàn bộ.
- Dùng structured event logs/traces cho sensor_id và request_id; áp dụng access control, retention, sampling thận trọng và correlation.

## 7. Wrong / tempting fixes
- Chỉ tăng quota metric: dời chi phí và điểm nghẽn, không giải quyết growth rate.
- Tăng CPU/replica API: không làm giảm distinct label combinations.
- Xóa tất cả labels: giảm series nhưng phá khả năng phân tích depot/outcome.
- Xóa event logs: tiết kiệm storage nhưng làm mất forensic và audit evidence.
- Sampling metric event không kèm chính sách series: vẫn có thể tạo rất nhiều series duy nhất.

## 8. Production implications
Thiết kế label allowlist; kiểm tra active-series và ingestion rejection theo thời gian; áp dụng quota/cardinality guard; giữ 7 ngày event log forensic theo chính sách, có tenant isolation, redaction và truy cập theo vai trò. So sánh dữ liệu dashboard trong canary trước cutover.

## 9. Trade-offs
Metric dimension càng chi tiết càng tăng khả năng drilldown và chi phí. Logs/traces phù hợp truy vết event nhưng cần retention, index cost và kiểm soát dữ liệu nhạy cảm. 5.000-series budget production phải được đo theo active-series thực, không suy trực tiếp từ fixture.

## 10. Senior engineer nên nhận ra
Không kết luận từ CPU hay event count đơn lẻ; xác định chính xác cardinality theo label combinations, kiểm tra yêu cầu observability, thiết kế bounded dimensions và kiểm chứng chi phí/độ tin cậy bằng canary cùng rollback.

## 11. Regression và rollout
Tạo synthetic fixtures ở nhiều quy mô, cảnh báo khi active series gần quota, kiểm tra cardinality theo tenant/region, so sánh dashboard và forensic query sau rollout. Rollback policy khi mất dimension bắt buộc; không rollback bằng cách bật lại high-cardinality request labels vô hạn.
