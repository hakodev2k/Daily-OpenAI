# UNIT-CAL-001 — Lịch kiểm tra inverter lệch sau tháng ngắn

## Mục tiêu
Điều tra lỗi lịch lặp theo tháng trong .NET, tái hiện triệu chứng, kiểm chứng giả thuyết bằng các fixture và sửa code với regression tests.

## Bối cảnh thực tế
Trang trại điện mặt trời lên lịch kiểm tra inverter mỗi tháng. Quy tắc: lấy **cùng ngày trong tháng như ngày kiểm tra đầu tiên**; nếu tháng không có ngày đó, chọn ngày cuối tháng. Tháng sau vẫn phải dựa trên ngày mong muốn ban đầu. Một bản cập nhật gần đây làm lịch tháng 3 và tháng 4 sớm hơn hợp đồng dù số kỳ không đổi, ứng dụng không có exception.

## Bạn cần làm gì
1. Chạy starter và reproduce, ghi ngày thực tế và ngày kỳ vọng.
2. Đưa ra ít nhất hai giả thuyết, phân biệt timezone, quy tắc calendar và state giữa các kỳ.
3. Sửa hàm `InspectionSchedule.Build` trong `starter/Program.cs` mà không sửa fixture.
4. Verify các chuỗi cuối tháng, năm nhuận, đổi năm, count bằng 0 và count âm.
5. So sánh reference solution và nêu trade-offs khi yêu cầu thay đổi.

## Yêu cầu môi trường
PowerShell 5.1+ hoặc PowerShell 7, .NET SDK 8.x; không cần database, Docker, cloud hay NuGet ngoài SDK.

## Chạy nhanh
Từ thư mục unit:
```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Script xác nhận **starter ban đầu** có sai lệch và trả exit code 0 khi triệu chứng xuất hiện. Sau khi sửa, baseline không còn; hãy chạy `./verify.ps1` để xác nhận code của bạn.

## Những gì cần quan sát
- So sánh kỳ bắt đầu 31/01/2026 với control 15/01/2026.
- Kiểm tra kỳ tháng 2 và các kỳ sau tháng 2, không chỉ tháng đầu bị rút ngắn.
- So sánh năm thường với năm nhuận và lịch qua tháng 12.
- Ghi bằng chứng tại [workspace](workspace/my-investigation.md) và [incident](evidence/incident.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler — chỉ mở sau khi tự làm:** [Reference Solution](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). `verify.ps1` chạy learner-editable `starter/`, không chạy reference.

## Estimated Time
45–60 phút. L2 Investigation / D4.
