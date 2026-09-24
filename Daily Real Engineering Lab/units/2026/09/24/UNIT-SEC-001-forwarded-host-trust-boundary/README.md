# UNIT-SEC-001 — Trusted Origin Behind a Reverse Proxy

## Mục tiêu
Điều tra một API chạy sau reverse proxy tạo link account-recovery không ổn định theo request metadata và xác định trust boundary phù hợp.

## Bối cảnh thực tế
Ứng dụng tạo absolute recovery link để gửi email. Ở môi trường production, proxy chuyển tiếp thông tin request tới ASP.NET Core. QA phát hiện cùng một account có thể nhận link với host khác dự kiến khi request đi qua một đường kiểm thử đặc biệt.

## Bạn cần làm gì
Reproduce, ghi hypotheses, xác định dữ liệu nào đang quyết định public origin, đề xuất và triển khai contract an toàn hơn mà không phá deployment sau proxy.

## Yêu cầu môi trường
.NET 8 SDK, PowerShell.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script chạy hai request metadata khác nhau qua cùng logic tạo link và kiểm tra public origin đầu ra.

## Những gì cần quan sát
So sánh hostname trong link, input nào làm output thay đổi, và dữ liệu đó có thuộc trust boundary của ứng dụng hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
Xem `hints/`.

## Reference Solution
Spoiler: xem `solution/README.md` sau khi tự thử.

## Expected Results
Xem `expected-results/`.

## Estimated Time
50 phút.