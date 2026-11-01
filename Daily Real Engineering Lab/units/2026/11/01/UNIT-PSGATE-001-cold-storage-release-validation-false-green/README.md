# UNIT-PSGATE-001 — Cold-storage release gate báo xanh dù validator thất bại

## Mục tiêu
Điều tra một CI release gate chạy bằng PowerShell; phân biệt kết quả kiểm tra thực tế với trạng thái thành công mà pipeline nhận được. Thu thập bằng chứng, sửa tại workspace của bạn, và kiểm thử cả đường thành công lẫn thất bại.

## Bối cảnh thực tế
Hệ thống báo cáo nhiệt độ kho lạnh cần chạy kiểm tra chất lượng trước mỗi lần phát hành. Nhóm vận hành nhận được một bản release màu xanh, nhưng log từ công cụ kiểm tra cho thấy có 2 hợp đồng dữ liệu không đạt. Việc phát hành nhầm có thể khiến báo cáo compliance sai và tăng thời gian xử lý sự cố.

## Bạn cần làm gì
1. Chạy kịch bản release giả lập ở chế độ thành công và thất bại.
2. Ghi nhận output của validator, trạng thái của wrapper và exit code mà tiến trình cha quan sát.
3. Đưa ra ít nhất hai giả thuyết, kiểm tra bằng chứng và sửa **chỉ** file `starter/invoke-quality-gate.ps1`.
4. Chạy `verify.ps1` để bảo đảm pipeline từ chối kết quả lỗi mà không làm hỏng đường thành công.

## Yêu cầu môi trường
Windows với `cmd.exe` và Windows PowerShell 5.1 hoặc PowerShell 7; không cần Docker, .NET SDK, Azure, NuGet hay tài khoản trả phí. Các script tự dùng cùng PowerShell engine với script cha.

## Chạy nhanh
Mở PowerShell tại thư mục unit:
```powershell
.\scripts\setup.ps1
.\scripts\run.ps1
.\scripts\reproduce.ps1
```

## Cách reproduce vấn đề
```powershell
.\scripts\reproduce.ps1
```
Script reproduction **PASS** khi xác nhận triệu chứng của bản starter (không có nghĩa là gate đúng). Để xem trực tiếp:
```powershell
.\starter\invoke-quality-gate.ps1 -Mode pass
.\starter\invoke-quality-gate.ps1 -Mode fail
```
Đọc thêm `evidence/incident.log` (dữ liệu synthetic).

## Những gì cần quan sát
- Validator in thông báo nào ở hai chế độ?
- Tiến trình cha nhận exit code nào trong trường hợp validator báo lỗi?
- Có dấu hiệu release gate báo thành công trái với bằng chứng không?
- Vì sao chỉ nhìn dòng cuối của console log có thể gây nhầm lẫn?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong `workspace/my-investigation.md`.
3. Thử fix ở `starter/`.
4. Verify bằng `scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ mở sau khi tự điều tra và thử sửa:** [Solution](solution/README.md)

## Expected Results
- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)
- `reproduce.ps1` trả 0 khi chứng minh được lỗi starter.
- `verify.ps1` trả nonzero với starter ban đầu; sau khi sửa đúng trả 0, kiểm tra cả hai nhánh pass/fail.

## Estimated Time
50 phút (D4 / L2). Không thay đổi `fixtures/` hoặc `scripts/verify.ps1` để làm xanh bài kiểm thử.
