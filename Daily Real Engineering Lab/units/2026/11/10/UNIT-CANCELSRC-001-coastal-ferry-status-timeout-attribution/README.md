# UNIT-CANCELSRC-001 — Ferry ETA cancellation attribution
## Mục tiêu
Phân tích kết quả hủy trong service .NET; sử dụng evidence để đưa ra quyết định vận hành.
## Bối cảnh thực tế
Dashboard tại bến phà báo `CLIENT_CANCELLED` tăng mạnh dù khách vẫn mở ứng dụng; alert downstream không được kích hoạt.
## Bạn cần làm gì
Reproduce, ghi hai giả thuyết, kiểm tra các nguồn tín hiệu, sửa `starter/Program.cs`, chạy verification.
## Yêu cầu môi trường
.NET SDK 8, PowerShell; không cần NuGet ngoài SDK hoặc cloud.
## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
```
## Cách reproduce vấn đề
`./reproduce.ps1` trả exit 0 nếu phát hiện sai lệch trong starter.
## Những gì cần quan sát
So sánh kết quả ba case: upstream chậm, caller chủ động hủy, upstream nhanh. Ghi actual/expected, ảnh hưởng đến SLO và alert.
## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.
## Hints
[01](hints/hint-01.md) · [02](hints/hint-02.md) · [03](hints/hint-03.md)
## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md)
## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)
## Estimated Time
55 phút; L2 / D4.
