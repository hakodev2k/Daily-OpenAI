# UNIT-RECORD-001 — Nhà kính liên tục nhận lại cấu hình tưới không thay đổi

## Mục tiêu
Reproduce một sai lệch trong vòng đồng bộ cấu hình thiết bị, phân biệt thay đổi thật với cập nhật dư thừa, và kiểm chứng hành vi khi dữ liệu đầu vào được tái sử dụng hoặc chỉnh sửa.

## Bối cảnh thực tế
Gateway của nhà kính nhận các bản chụp cấu hình tưới từ hệ thống quản lý. Mỗi lần gateway xác định cấu hình khác bản đã áp dụng, nó gửi một lệnh cập nhật xuống bộ điều khiển. Khi chạy mô phỏng, cùng một cấu hình được gửi nhiều lần dù dashboard không ghi nhận thay đổi nghiệp vụ. Ngược lại, một thao tác cập nhật dữ liệu trong bộ nhớ có thể không phát sinh lệnh mong đợi.

## Bạn cần làm gì
1. Chạy starter và ghi số lần áp dụng cấu hình.
2. Đưa ra ít nhất hai giả thuyết; quan sát bản chụp đầu vào và trạng thái đã lưu.
3. Chỉ sửa **starter/CalibrationSync.cs**, không sửa harness.
4. Chạy verify để bảo đảm không gửi lệnh thừa nhưng vẫn gửi khi nội dung thực sự thay đổi.
5. So sánh với reference solution sau khi đã tự thử.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell (Windows PowerShell 5.1 hoặc PowerShell 7). Không cần Docker, database, Azure hay NuGet package ngoài .NET SDK.

## Chạy nhanh
~~~powershell
./setup.ps1
./reproduce.ps1
~~~

## Cách reproduce vấn đề
Chạy **./reproduce.ps1** với starter ban đầu. Exit code 0 nghĩa là symptom đã được tái hiện; script in số lệnh đã gửi. Sau khi tự sửa, chạy **./verify.ps1**. Trước khi sửa, verify phải thất bại ở các contract quan trọng.

## Những gì cần quan sát
Ghi lại số lần gateway phát lệnh cho một zone khi các lần polling có cùng revision và cùng dãy ngưỡng. Thử cả trường hợp đầu vào được tạo mới và trường hợp nguồn dữ liệu thay đổi sau lần quan sát đầu tiên. Phân biệt thay đổi nội dung và thay đổi đối tượng trong bộ nhớ.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Verify correctness và regression.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) — chỉ mở sau khi tự điều tra.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time
35 phút. Có thể ghi chép tại [workspace/my-investigation.md](workspace/my-investigation.md).
