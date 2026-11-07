# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms
Báo cáo 5 phút lệch trong khi tổng net flow của ca đúng. Lỗi chỉ lộ ra khi đối chiếu bucket, không lộ qua kiểm tra \`Sum(Delta)\`.

## 2. Evidence
Event A-02 xảy ra lúc 10:04:55 UTC nhưng đến lúc 10:05:07; A-04 xảy ra lúc 10:04:59 nhưng đến 10:05:22. A-05 xảy ra 10:09:59 nhưng đến 10:10:15. Chúng làm lệch các cửa sổ sát ranh giới. Dữ liệu đủ sáu ID, không có retry.

## 3. Root cause
Starter phân nhóm theo \`ReceivedAt\` (ingestion time) thay vì \`OccurredAt\` (event time). Thứ tự xử lý theo thời điểm nhận là hợp lý cho pipeline, nhưng không quyết định được business window. Vì vậy total vẫn đúng nhưng phân bổ theo khung sai.

## 4. Why the fix works
[WindowRollup.cs](WindowRollup.cs) dùng thời điểm event xảy ra, chuẩn hóa sang UTC trước khi làm tròn xuống bội số 5 phút. \`DateTimeOffset\` biểu diễn instant kèm offset; \`UtcDateTime.Ticks\` bảo đảm cùng instant luôn cùng bucket. Bucket là [start, start+5 phút).

## 5. How to verify
Chép \`solution/WindowRollup.cs\` đè \`starter/WindowRollup.cs\` **sau khi đã hoàn tất lần thử riêng**, chạy \`./verify.ps1\`. Kiểm tra cả hai fixture và total. \`reproduce.ps1\` sau fix có thể exit 1 vì symptom không còn, đó là đúng contract.

## 6. Alternative fixes
- Phân nhóm trực tiếp theo event time trong truy vấn database khi dữ liệu gốc được lưu đầy đủ: phù hợp báo cáo batch, đổi lại chi phí truy vấn và index.
- Với stream liên tục, dùng event-time windows, watermark và chính sách late arrivals: giảm chi phí chốt window nhưng cần hợp đồng rõ về correction/retraction.
- Nếu dashboard là **thời điểm nhận**, dùng ingestion time là hợp lệ; phải đổi tên metric và product contract thay vì giả định là thời điểm xe đi qua.

## 7. Wrong or tempting fixes
- Sắp xếp lại theo \`ReceivedAt\`: không sửa ý nghĩa bucket.
- Tăng worker replicas: không sửa phân loại thời gian.
- Chỉnh offset bằng cộng cố định 7 giờ: phá hỏng các input UTC và vùng giờ khác.
- Bỏ các event đến muộn: làm sai cả tổng, không còn bảo toàn dữ liệu.
- Chỉ kiểm tra tổng ca: bỏ sót sai lệch theo window.

## 8. Production implications
Báo cáo đã chốt phải có chính sách late arrival: ví dụ giữ raw immutable, cho phép correction versioned trong 30 phút sau khi chốt, phát hành \`windowVersion\` và audit delta. Sau hạn đó, tạo adjustment kỳ sau hoặc manual reconciliation. Cần phân biệt arrival delay với sensor clock skew, giám sát độ trễ và không âm thầm sửa báo cáo tài chính.

## 9. Trade-offs
Chờ watermark dài làm giảm số correction nhưng tăng độ trễ báo cáo; chốt nhanh cải thiện latency nhưng tăng chi phí reconciliation. Event time đáng tin chỉ khi sensor clocks được quản trị; đồng bộ clock và giám sát skew là điều kiện vận hành. Dùng UTC để tránh lỗi offset/DST; format local time chỉ ở lớp hiển thị.

## 10. What a Senior engineer should notice
Một hệ thống có thể **đúng tổng nhưng sai phân bố**. Contract phải nói rõ loại thời gian, time zone, cửa sổ half-open, xử lý event trễ và tính bất biến của raw events. Test cần có event sát boundary, offset khác nhau và arrival out-of-order; không kiểm thử chỉ happy path.

## Giới hạn lab
Fixture mô phỏng pipeline in-memory, không thực hiện watermark/persistent correction. Đây là phần quyết định vận hành cần thiết kế riêng trước production.
