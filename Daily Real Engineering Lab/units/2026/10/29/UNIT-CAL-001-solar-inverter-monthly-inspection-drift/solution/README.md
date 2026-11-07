# Reference Solution — xem sau khi reproduce và tự sửa

## 1. Symptoms
Lịch từ 31/01 đúng ở tháng 2 nhưng sớm ở tháng 3 trở đi; lịch ngày 15 không lệch.

## 2. Evidence
Starter cho 31/01, 28/02, 28/03, 28/04, 28/05 thay vì 31/01, 28/02, 31/03, 30/04, 31/05. Năm nhuận chỉ thay ngày tháng 2, không thay mốc gốc tháng 3.

## 3. Root cause
`DateOnly.AddMonths` clamp ngày khi tháng đích ngắn hơn. Starter cập nhật `due` từ kết quả kỳ trước, nên sau tháng 2 ngày 28/29 bị coi là ngày mong muốn mới. Lỗi là mất calendar anchor, không phải lỗi .NET.

## 4. Why the fix works
Tính tháng đích từ tháng gốc và offset, chọn ngày `Math.Min(firstDue.Day, DateTime.DaysInMonth(year, month))`. Ngày mong muốn ban đầu không thay đổi. Xem [ReferenceScheduler.cs](ReferenceScheduler.cs).

## 5. How to verify
Thay riêng method trong `starter/Program.cs`, chạy `./verify.ps1` từ unit root. Test năm thường, năm nhuận, ngày 30, ngày 15, đổi năm, count=0 và count âm. Không chạy reference thay cho learner code.

## 6. Alternative fixes
`firstDue.AddMonths(offset)` cho mỗi offset độc lập cũng đúng với contract này. Tách tháng đích và ngày mong muốn giúp mở rộng chính sách dễ hơn.

## 7. Wrong / tempting fixes
- Cộng 30 ngày: tháng dài ngắn khác nhau.
- Cộng bù ba ngày vào tháng 3: không bao phủ năm nhuận và ngày 30.
- Đổi timezone/UTC: không sửa lỗi lịch calendar month.
- Sửa hiển thị: không sửa dữ liệu lịch.
- Chạy lại job: có thể tạo lịch sai lặp lại.

## 8. Production implications
Lưu ngày gốc và version của quy tắc recurrence; khi thay đổi chính sách phải audit lịch đã công bố. Nếu có quy tắc ngày làm việc hoặc timezone, xác định rõ ngày địa phương trước khi áp dụng calendar rule.

## 9. Trade-offs
Anchor-based calculation deterministic, ít state nhưng cần giữ ngày mong muốn ban đầu. Lịch “30 ngày sau lần kiểm tra thực tế” là yêu cầu khác, không dùng thuật toán này.

## 10. Senior engineer insight
Một phép tính đúng từng bước vẫn có thể sai khi lặp nhiều lần vì mất domain invariant. Test phải kiểm tra **kỳ sau tháng ngắn**, không chỉ tháng ngắn.
