# UNIT-ETAGW-001 — Hai biên tập viên cùng sửa hồ sơ phục hồi đất ngập nước

## Mục tiêu
Điều tra mất cập nhật khi hai người chỉnh sửa cùng một tài nguyên, thu thập evidence tại HTTP write boundary, rồi viết regression contract cho tình huống xung đột.

## Bối cảnh thực tế
Cổng quản lý dự án phục hồi đất ngập nước cho phép cán bộ A và B cùng mở bản nháp kế hoạch. A lưu thay đổi trước; B vẫn mở bản cũ và lưu sau. Hệ thống báo cả hai thao tác thành công, nhưng ghi chú mới của A biến mất. Không có lỗi 500 hay timeout.

## Bạn cần làm gì
1. Chạy starter và ghi lại validator của hai GET, status của hai lần PUT, nội dung cuối.
2. Đề xuất ít nhất ba hypothesis: UI giữ state cũ, persistence bị lỗi, hoặc hợp đồng cập nhật không được thực thi.
3. Truy vết đường đọc → sửa → ghi; xác định invariant cần bảo vệ.
4. Sửa trực tiếp starter/Program.cs, chạy verification; giữ nguyên hành vi cập nhật hợp lệ.
5. Đối chiếu reference solution và đánh giá việc chuyển sang SQL/EF Core đa instance.

## Yêu cầu môi trường
Windows PowerShell 5.1+ hoặc PowerShell 7; .NET 8 SDK. Không cần database, cloud hay NuGet bên ngoài. Bài tập mô phỏng HTTP response tại service boundary, không khởi chạy web server.

## Chạy nhanh
~~~powershell
./setup.ps1
./reproduce.ps1
~~~

## Cách reproduce vấn đề
~~~powershell
./run.ps1
~~~
Reproduce thành công nghĩa là lỗi được quan sát, **không** có nghĩa đã fix.

## Những gì cần quan sát
- Hai người đang làm việc với cùng một hay hai revision khác nhau?
- Hai response có phù hợp với nội dung cuối không?
- Dữ liệu mới hơn có bị ghi đè mà không có conflict response?
- Dashboard 5xx bằng 0 có đủ chứng minh dữ liệu an toàn không?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong workspace.
3. Thử fix trong starter/.
4. Chạy ./verify.ps1 và ghi bằng chứng.
5. Chỉ sau đó mới xem solution/.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải và trade-offs](solution/README.md) · [Reference code](solution/Program.cs)

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
55 phút. [Incident timeline](evidence/incident-timeline.md) · [Investigation workspace](workspace/my-investigation.md)
