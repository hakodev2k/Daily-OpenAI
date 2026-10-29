# Reference Solution — spoiler

**Chỉ đọc sau khi đã reproduce, ghi hypotheses và thử fix trong `starter/ReportBuilder.cs`.**

## 1. Symptoms
Báo cáo hiển thị 2 phiếu, 700 kg, average 350 kg. Không có exception; số kg đều là decimal hợp lệ.

## 2. Evidence
Fixture mở snapshot lần 1 với hai phiếu (100+200 kg), lần 2 với ba phiếu (100+200+400 kg). `SnapshotOpens=2`. Count khớp S1; total khớp S2.

## 3. Root cause
`IEnumerable<Receipt>` là mô tả **cách duyệt**, không cam kết dữ liệu đã được snapshot. `Count()` và `Sum()` là hai terminal operations, nên source được duyệt hai lần. Gateway mô phỏng mở snapshot ở thời điểm bắt đầu mỗi lượt duyệt. Report vô tình ghép hai thời điểm quan sát.

## 4. Why the fix works
Reference implementation duyệt `ReadCurrent()` một lần, cập nhật count và total trong cùng `foreach`. Do đó các chỉ số dùng chung một snapshot. `checked` ngăn tràn count/decimal bị bỏ qua. Không cần giữ toàn bộ dữ liệu trong RAM.

## 5. How to verify
Thay `starter/ReportBuilder.cs` bằng nội dung của file reference, chạy `./verify.ps1`. Cần ba PASS: live consistency, empty source, stable decimal source. `./reproduce.ps1` sau fix dự kiến trả nonzero vì symptom đã biến mất; đó không phải regression.

## 6. Alternative fixes
`var snapshot = feed.ReadCurrent().ToArray();` rồi tính `Count` và `Sum` trên array cũng chỉ đọc upstream một lần, phù hợp khi cần nhiều phép truy vấn và volume có giới hạn. Streaming fold tiết kiệm O(n) bộ nhớ, thích hợp báo cáo lớn. Nếu source thật là database với nhiều query, một lượt enumeration trong process **không tự động** tạo transaction-level snapshot giữa các query; cần xác định isolation và data-consistency contract của database.

## 7. Wrong / tempting fixes
- Thay `decimal` bằng `double`: không giải quyết khác biệt snapshot, còn giảm độ chính xác.
- Thêm `Distinct()`: dữ liệu không bị trùng ID trong fixture.
- Retry `Count()` hoặc `Sum()`: tăng số lần đọc và có thể làm mismatch nặng hơn.
- Lock riêng `ReportBuilder`: không khóa được producer bên ngoài hoặc các replica khác.
- Cache toàn bộ feed vô hạn: có thể che triệu chứng nhưng tăng memory và không xác định snapshot policy.

## 8. Production implications
Gắn snapshot/version/watermark vào báo cáo; đo số lần mở nguồn, thời gian đọc, late arrivals và mismatch. Với API paging, yêu cầu stable cursor hoặc snapshot token; một vòng lặp trên pages vẫn có thể nhìn thấy dữ liệu thay đổi giữa pages nếu upstream không bảo đảm consistency.

## 9. Trade-offs
Streaming: O(1) memory, chỉ tính được các aggregate hỗ trợ online fold. Materialization: O(n) memory, tái sử dụng được nhiều phép tính. Database snapshot: consistency mạnh hơn nhưng có chi phí transaction/version retention và giới hạn thời gian giữ snapshot.

## 10. Senior engineer nên nhận ra
Chứng minh contract của data source trước khi tối ưu LINQ. Kiểm tra đơn vị consistency (record, page, transaction, report), phân biệt deterministic fixture với production isolation, và thiết kế regression tests theo **hành vi** chứ không theo tên API cụ thể.
