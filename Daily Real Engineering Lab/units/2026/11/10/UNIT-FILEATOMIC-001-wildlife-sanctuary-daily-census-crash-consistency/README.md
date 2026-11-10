# UNIT-FILEATOMIC-001 — Báo cáo kiểm kê động vật bị hỏng sau khi tiến trình dừng giữa lúc ghi

## Mục tiêu
Điều tra tính toàn vẹn của bản báo cáo đã được xác nhận khi quá trình refresh gặp lỗi; thiết kế và chứng minh recovery contract bằng failure injection.

## Bối cảnh thực tế
Khu bảo tồn ghi báo cáo CSV kiểm kê động vật mỗi ngày. Một job refresh gặp sự cố I/O ngắn; sau đó hệ thống báo cáo vẫn có file nhưng dữ liệu không còn đầy đủ. Đội vận hành cần bảo đảm bản cuối cùng đã được xác nhận vẫn đọc được khi refresh thất bại.

## Bạn cần làm gì
Chạy mô phỏng, ghi lại bằng chứng, đề xuất tối thiểu hai giả thuyết và kiểm chứng chúng. Chỉnh sửa **starter/Program.cs** để một lần refresh thất bại không làm mất báo cáo đã xác nhận. Giữ nguyên contract thành công và cleanup.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell; không cần database, network, NuGet package bên ngoài.

## Chạy nhanh
Từ thư mục unit:
```powershell
./scripts/setup.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
`./scripts/reproduce.ps1` chạy ứng dụng gốc với fault injection cố định. Script thành công khi phát hiện **contract thất bại như mong đợi**; đây không phải là verify bản sửa.

## Những gì cần quan sát
So sánh `INJECTED_FAULT`, `OLD_REPORT_PRESERVED`, `OBSERVED_BYTES`, `NEW_REPORT_COMMITTED` và `NO_ORPHAN_TEMP_FILES`. Quan sát file còn tồn tại không đồng nghĩa với dữ liệu còn hợp lệ. Ghi hypothesis trước khi xem gợi ý.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix ở starter.
4. Chạy `./scripts/verify.ps1` (phải kiểm tra starter đã sửa).
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham khảo](solution/README.md)

## Expected Results
Trước fix: `CONTRACT=FAIL` và `OLD_REPORT_PRESERVED=false`; sau fix: `CONTRACT=PASS`, các điều kiện đều true. Cả hai lần chạy đều có injected fault theo chủ đích.

## Estimated Time
50 phút.
