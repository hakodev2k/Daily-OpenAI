# UNIT-CFG-004 — Environment Configuration Key Path

## Mục tiêu
Điều tra một lỗi cấu hình chỉ lộ ra sau deployment và xác định vì sao giá trị environment-specific không trở thành effective configuration.

## Bối cảnh thực tế
Một background export service dùng batch size mặc định `100` ở local. Deployment production đã cấu hình batch size `25` để giảm áp lực lên downstream, nhưng log khởi động vẫn báo `100`.

## Bạn cần làm gì
Reproduce symptom, xác nhận process nhận configuration từ environment, hình thành hypothesis về configuration pipeline, sửa starter và verify rằng giá trị deployment được sử dụng.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell

## Chạy nhanh
```powershell
cd "Daily Real Engineering Lab/units/2026/09/15/UNIT-CFG-004-environment-key-path"
./reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` cung cấp batch size `25` qua environment rồi chạy starter ở probe mode. Script chỉ PASS khi symptom ban đầu xuất hiện: ứng dụng vẫn báo effective value `100`.

## Những gì cần quan sát
- Giá trị mà deployment cung cấp.
- Giá trị effective mà application báo ra.
- Cách configuration được truy cập trong request path khởi tạo.
- Không thay đổi fallback trước khi xác định vì sao override không có hiệu lực.

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
[Spoiler — Reference Solution](solution/README.md)

## Expected Results
Before: deployment cung cấp `25`, application báo `100`.

After: cùng deployment input, learner-edited starter báo `25`; `verify.ps1` PASS.

Nếu không reproduce được, chạy `dotnet --version`, xác nhận .NET 8 SDK khả dụng và chạy script từ unit root.

## Estimated Time
30–45 phút.
