# UNIT-ASYNC-001 — Báo cáo đồng bộ dữ liệu giao thông bỏ sót sự cố đối tác

## Mục tiêu
Điều tra một batch async có nhiều kết quả độc lập, thu thập bằng chứng trước khi thay đổi code, và thiết kế báo cáo phân biệt thành công, lỗi từng nguồn và hủy tác vụ.

## Bối cảnh thực tế
Hệ thống tổng hợp thời gian đến của xe buýt từ nhiều nhà vận hành. Một đêm, dashboard vận hành chỉ hiển thị một lỗi tổng quát và không có bản ghi thành công, dù telemetry tại từng nguồn cho thấy nhiều kết quả khác nhau. Đội trực vận hành không biết nên liên hệ đối tác nào trước.

## Bạn cần làm gì
1. Chạy starter và ghi lại các nguồn dữ liệu, kết quả quan sát được và kỳ vọng.
2. Nêu ít nhất ba giả thuyết; phân biệt lỗi mạng, mất dữ liệu báo cáo và hủy tác vụ.
3. Điều tra **starter/BatchRunner.cs** cùng dữ liệu sự cố tại **evidence/incident.md**.
4. Sửa đường đi của **starter/** để kết quả báo cáo phản ánh từng nguồn mà không đánh mất semantics của cancellation.
5. Chạy verify trên chính starter đã sửa, sau đó mới so sánh solution.

## Yêu cầu môi trường
.NET SDK 10 (tối thiểu 10.0.100, roll-forward trong major 10), PowerShell 7 hoặc Windows PowerShell. Không cần Azure, database, Docker hay NuGet ngoài SDK.

## Chạy nhanh
~~~powershell
./setup.ps1
./run.ps1
~~~
Hoặc:
~~~powershell
dotnet run --project ./starter/BatchFaultLab.csproj -- --demo
~~~

## Cách reproduce vấn đề
~~~powershell
./reproduce.ps1
~~~
Lệnh này xác nhận **triệu chứng của starter gốc**; sau khi sửa code, lệnh này có thể thất bại vì sự cố đã biến mất. Để kiểm tra bản sửa, dùng **./verify.ps1**.

## Những gì cần quan sát
- Tổng số nguồn thành công/thất bại trong báo cáo có khớp sự kiện giả lập không?
- Một nguồn thất bại có làm biến mất kết quả từ nguồn khác không?
- Khi hai nguồn cùng thất bại, dashboard có đủ thông tin định tuyến sự cố không?
- Khi batch bị hủy, hệ thống đang báo hủy hay coi đó là lỗi của đối tác?
- Nếu không reproduce được, kiểm tra SDK, working directory và chắc chắn starter chưa được sửa.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md).

## Reference Solution
⚠️ Spoiler: chỉ mở [giải pháp tham khảo](solution/README.md) sau khi tự điều tra và verify.

## Expected Results
[Trước sửa](expected-results/before.md) · [Sau sửa](expected-results/after.md). **./verify.ps1** phải báo 6/6 PASS trên **starter đã sửa**, không phải chỉ trên solution.

## Estimated Time
55–70 phút. Ghi giả thuyết và bằng chứng tại [workspace/my-investigation.md](workspace/my-investigation.md).
