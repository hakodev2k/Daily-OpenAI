# UNIT-CICD-001 — Publish Artifact Boundary

## Mục tiêu
Điều tra vì sao một release package có thể khác với output đã được CI build thành công, rồi thiết lập artifact boundary có thể tái lập và kiểm chứng.

## Bối cảnh thực tế
Một internal Order API luôn build xanh. Tuy nhiên một số release sau khi deploy lại thiếu file runtime cần thiết hoặc mang theo file từ lần build trước. Rollback cũng không hoàn toàn tái tạo đúng package đã từng chạy.

## Bạn cần làm gì
Reproduce package drift, thu thập bằng chứng từ workspace và package, ghi hypothesis, sửa pipeline mô phỏng trong `starter/`, sau đó verify rằng cùng một publish output được promote nguyên vẹn.

## Yêu cầu môi trường
- .NET SDK 8.0.x
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Script chuẩn bị một workspace giống CI, build/publish ứng dụng và tạo deployment package theo logic hiện tại. Reproduction thành công khi script phát hiện package không khớp manifest của output dự kiến.

## Những gì cần quan sát
- Danh sách file trong publish output.
- Danh sách file trong deployment package.
- Hash manifest của hai tập file.
- Package có thay đổi hay không khi workspace chứa file cũ không thuộc release hiện tại.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi đã thử](solution/README.md)

## Expected Results
Trước fix, deployment package có thể khác publish manifest khi workspace chứa residue. Sau fix, `verify.ps1` phải báo `VERIFY_PASS`, package manifest phải bằng publish manifest và việc thêm residue vào workspace không làm thay đổi package.

## Estimated Time
45 phút.
