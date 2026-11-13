# UNIT-UPLOADPATH-001 — Thiết kế đường upload bài thi khi 12.000 sinh viên nộp cùng lúc

## Mục tiêu
Ra quyết định kiến trúc upload có thể bảo vệ bằng số liệu, security boundary và failure recovery; phân biệt upload thành công với bài thi đã được chấp nhận.

## Bối cảnh thực tế
Cổng thi trực tuyến hiện gửi PDF qua ASP.NET Core API, sau đó API lưu Azure Blob. Trong 10 phút trước hạn chót, người dùng phản ánh timeout, nộp lại nhiều lần và không biết bài đã được ghi nhận hay chưa. Không được kéo dài deadline.

## Bạn cần làm gì
1. Dựa trên dữ liệu trong [evidence/capacity.md](evidence/capacity.md), tính tải trung bình và peak, xác định các giả định cần xác minh.
2. So sánh ít nhất ba phương án trong [workspace/decision.md](workspace/decision.md).
3. Vẽ sequence upload/finalize, định nghĩa idempotency, authorization, malware scanning, expiry, quota và cleanup.
4. Lập rollout, observability, rollback và một bảng test lỗi.
5. Chỉ sau khi hoàn tất mới đối chiếu [solution/README.md](solution/README.md).

## Yêu cầu môi trường
Không cần Azure subscription. Chỉ cần Markdown editor và máy tính. Đây là L5 design-decision lab, không có executable starter; dữ liệu được cố định để replay quyết định.

## Chạy nhanh
Mở evidence, copy template workspace và điền các phép tính; không cần setup cloud.

## Cách reproduce vấn đề
Dùng tải ở evidence để ước lượng số upload đồng thời và băng thông cần thiết; đối chiếu với API gateway, API worker và storage budget đã cho. Ghi rõ chỗ thiếu dữ liệu, không giả định toàn bộ request đồng thời nếu chưa mô hình hóa.

## Những gì cần quan sát
Độ lớn file p50/p95, số request trong peak, connection duration, bộ nhớ theo upload, timeout rate, trạng thái accepted vs uploaded, chi phí và blast radius.

## Quy tắc làm lab
1. Reproduce bằng capacity replay trước.
2. Ghi ít nhất ba hypothesis.
3. Đề xuất phương án cùng risk.
4. Verify bằng acceptance matrix.
5. Chỉ sau đó xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
[Spoiler — reference decision](solution/README.md)

## Expected Results
Có capacity worksheet có đơn vị; decision record chỉ rõ điều kiện lựa chọn và trade-off; test plan bao gồm retry, expired credential, oversized file, abandoned upload, race khi finalize và outage của scanner. Không yêu cầu một kiến trúc duy nhất.

## Estimated Time
55 phút.
