# Reference Solution — Chỉ mở sau khi tự điều tra

## 1. Symptoms

Ca sáng worker nhận 3 túi hợp lệ, ca chiều nhận 1 túi, nhưng báo cáo cuối ca đều là 0.

## 2. Evidence

processed và worker khớp nhau; report khác worker. Quét trùng và mã không đọc được vẫn được lọc đúng. Ca rỗng không có chênh lệch.

## 3. Root cause

AcceptedScanCounter là mutable struct. Khi truyền vào constructor ShiftScanner(IAcceptedScanCounter), struct được boxing thành một bản sao. ShiftScanner.Increment() thay đổi bản boxed, trong khi ShiftService đọc counter.Value trên biến struct ban đầu. Vì vậy worker và report nhìn hai trạng thái khác nhau.

## 4. Why the fix works

Trong reference source, AcceptedScanCounter là sealed class, giữ nguyên interface và các hợp đồng khác. Cả worker và ShiftService cùng tham chiếu một instance; thay đổi trạng thái được quan sát nhất quán. Chỉ một dòng khai báo kiểu thay đổi; xem [reference code](LaundryAudit/ShiftAudit.cs).

## 5. How to verify

Chỉ sau khi tự thử, so sánh starter/LaundryAudit/ShiftAudit.cs với reference code. Chạy ./verify.ps1: ca sáng 3/3/3, ca chiều 1/1/1, ca rỗng 0/0/0. Contract chạy code trong starter, không chạy solution.

## 6. Alternative fixes

Một thiết kế ít mutable shared state hơn là để Process trả về số lượng accepted, sau đó tạo báo cáo trực tiếp từ kết quả. Khi chọn cách này phải bảo đảm report và worker đều dựa trên cùng một kết quả authoritative, không cộng đếm hai lần. Một phương án khác là immutable counter trả về giá trị mới và được gán lại rõ ràng ở một nơi sở hữu.

## 7. Wrong / tempting fixes

- Tăng bộ đếm report thủ công sau mỗi scan: có nguy cơ hai nguồn sự thật và lệch khi quy tắc thay đổi.
- Bỏ kiểm tra quét trùng: thay đổi business rule nhưng không giải quyết vấn đề trạng thái.
- Dùng static global counter: có thể làm một ca trông đúng nhưng gây rò trạng thái giữa các ca.
- Thêm lock quanh struct hiện tại: lock không hợp nhất hai bản sao đã tồn tại.

## 8. Production implications

Sai lệch này ảnh hưởng đối soát hóa đơn, số liệu SLA và điều tra thất thoát. Thêm contract test ở ranh giới worker/report; log correlation theo ca nhưng tránh lộ dữ liệu cá nhân.

## 9. Trade-offs

Class có reference semantics nhưng cần kiểm soát lifetime, concurrency và thread safety. Value type vẫn tốt cho dữ liệu bất biến hoặc giá trị truyền theo bản sao. Không nên chuyển tất cả struct thành class chỉ vì lab này.

## 10. Senior engineer should notice

Phân biệt lỗi business rule với lỗi ownership/aliasing; không coi worker log là bằng chứng đủ nếu báo cáo đọc nguồn khác; thiết kế một nguồn sự thật và kiểm chứng độc lập nhiều ca. Nếu mở rộng đa luồng, Increment() hiện tại chưa thread-safe, nên phải xem lại cơ chế đồng bộ trước khi đưa lên production.
