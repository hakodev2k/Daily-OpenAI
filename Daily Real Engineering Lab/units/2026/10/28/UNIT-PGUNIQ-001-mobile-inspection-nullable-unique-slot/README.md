# UNIT-PGUNIQ-001 — Lịch kiểm định lưu động bị đặt trùng dù bảng đã có UNIQUE

## Mục tiêu
Điều tra một vi phạm quy tắc đặt lịch ở database, thu thập bằng chứng SQL, sửa ràng buộc tại tầng dữ liệu và kiểm tra regression trên các trường hợp biên.

## Bối cảnh thực tế
Trung tâm kiểm định quản lý lịch theo depot, bay và thời điểm bắt đầu. Bay cố định có mã; ca kiểm định lưu động không gắn bay cố định. Quy tắc nghiệp vụ: tại cùng depot và thời điểm, một bay (kể cả nhóm lưu động) chỉ được có một lịch. Sau đợt nhập dữ liệu đối tác, hệ thống ghi nhận hai ca lưu động cùng thời điểm dù database đã khai báo ràng buộc duy nhất. Các ca ở bay cố định vẫn được bảo vệ.

## Bạn cần làm gì
- Reproduce tình huống bằng fixture, ghi lại hàng dữ liệu và định nghĩa constraint.
- Nêu ít nhất hai giả thuyết; kiểm tra semantics của PostgreSQL trước khi sửa.
- Chỉnh trực tiếp starter/01-schema.sql để bảo vệ invariant mà vẫn cho phép các lịch hợp lệ.
- Chạy verify.ps1 trên starter đã chỉnh; giải thích cách triển khai thay đổi khi bảng production đã có dữ liệu trùng.

## Yêu cầu môi trường
Docker Engine và Docker Compose v2 có hỗ trợ up --wait; PowerShell 7 (pwsh) hoặc Windows PowerShell 5.1. Không cần tài khoản cloud. Image PostgreSQL được pin tại postgres:16.4-alpine.

## Chạy nhanh
Từ thư mục unit:

~~~powershell
pwsh ./reproduce.ps1
~~~

Trên Windows PowerShell có thể chạy ./reproduce.ps1 nếu execution policy cho phép. Script khởi tạo schema và nạp fixture lại mỗi lần.

## Cách reproduce vấn đề
Chạy lệnh trên, quan sát NOTICE về hai lịch lưu động tại cùng depot và giờ. Sau đó chạy:

~~~powershell
pwsh ./verify.ps1
~~~

Với starter ban đầu, verify phải thất bại. Đây là hành vi dự kiến của bài lab, không phải lỗi Docker.

## Những gì cần quan sát
- Những hàng có cùng depot, thời gian, và nhóm bay.
- Sự khác biệt giữa trường hợp bay cố định và nhóm lưu động.
- Định nghĩa constraint hiện tại qua psql hoặc schema SQL.
- Trường hợp hợp lệ: khác depot, khác bay hoặc khác thời điểm vẫn phải được ghi.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào workspace/my-investigation.md.
3. Thử fix trong starter/01-schema.sql.
4. Verify bằng verify.ps1 (không phải bằng code solution).
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** Chỉ mở [Reference Solution](solution/README.md) sau khi đã tự điều tra và thử sửa.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). Script reproduce chứng minh triệu chứng; script verify chứng minh invariant đã được bảo vệ và các lịch hợp lệ không bị chặn.

## Estimated Time
50–65 phút. Không có bước nào yêu cầu dịch vụ trả phí.
