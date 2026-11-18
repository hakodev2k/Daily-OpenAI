# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms
Gateway B-22 gửi phép đo mới nhưng hệ thống báo STALE; A-17 bình thường, A-18 FUTURE.

## Evidence
A-17 epoch=1800000000000; B-22 epoch=1800000000. Cả hai đại diện cùng một instant khi diễn giải đúng hợp đồng nguồn. Giá trị B-22 khi đọc như milliseconds trỏ về năm 1970.

## Root cause
Gateway B gửi Unix **seconds**, còn A gửi Unix **milliseconds**. Starter gọi `FromUnixTimeMilliseconds` cho cả hai; không có lỗi exception, dẫn đến sai phân loại.

## Why the fix works
Chọn `FromUnixTimeSeconds` cho B và `FromUnixTimeMilliseconds` cho A dựa trên protocol đã khai báo. Không tự đoán bằng số chữ số.

## Verification
Chạy `./verify.ps1` trên starter đã sửa: A-17 ACCEPT, B-22 ACCEPT, A-18 FUTURE. Có thể chạy `dotnet run --project solution/Telemetry.csproj` để đối chiếu. Thêm test cho gateway không rõ, giá trị ngoài phạm vi, epoch âm, thời điểm ranh giới 10 phút, skew cho phép và dữ liệu đảo thứ tự.

## Alternative fixes
Chuẩn hóa timestamp thành ISO-8601 UTC ở ingress hoặc chuyển mọi event thành Unix milliseconds với schema version rõ ràng. Cần migration và contract test cho mọi producer.

## Wrong fixes
Nhân mọi timestamp nhỏ hơn 10^11 với 1000 là heuristic không có hợp đồng, dễ sai với lịch sử hoặc dữ liệu tương lai. Bỏ kiểm tra STALE chỉ che dấu lỗi. Thay `DateTimeOffset.UtcNow` không xử lý nhầm đơn vị.

## Production implications
Ghi metric số packet bị từ chối theo gateway, structured log rawEpoch + declaredUnit + normalizedUtc, không log dữ liệu nhạy cảm. Quarantine event sai hợp đồng thay vì âm thầm làm mất cảnh báo.

## Trade-offs
Explicit gateway mapping đơn giản nhưng cần cập nhật khi firmware thay đổi. Versioned schema bền vững hơn nhưng yêu cầu phối hợp giữa các đội; validation range nên phụ thuộc use case, không hardcode tùy tiện.

## Senior engineering insight
Đơn vị đo là một phần của API contract như currency, timezone hay byte length. Phân biệt sai dữ liệu (invalid) và dữ liệu hợp lệ nhưng cũ (stale); đảm bảo alert pipeline không âm thầm biến lỗi protocol thành quyết định vận hành.
