# UNIT-HTTP-003 — Requests Stall While the Downstream Service Is Healthy

## Mục tiêu

Điều tra một HTTP integration có throughput tụt mạnh dù downstream service vẫn trả `200`, CPU thấp và không có exception rõ ràng. Bạn cần reproduce, thu thập evidence về request progression, xác định resource boundary bị giữ quá lâu và sửa `starter/` mà không tăng connection limit để che triệu chứng.

## Bối cảnh thực tế

Một internal API kiểm tra metadata của tài liệu từ document service trước khi quyết định bước xử lý tiếp theo. Sau một thay đổi nhằm giảm memory allocation khi tải response lớn, production bắt đầu xuất hiện request bị timeout theo từng cụm. Downstream dashboard vẫn cho thấy service khỏe và hai request đầu tiên trả header gần như ngay lập tức.

Business impact: worker xử lý tài liệu bị nghẽn, queue tăng dần và team nghi ngờ network hoặc downstream service dù evidence ban đầu không khớp.

## Bạn cần làm gì

1. Chạy `reproduce.ps1` và ghi lại thứ tự event.
2. Xác nhận hai request đầu nhận response header thành công.
3. Quan sát request thứ ba và so sánh với giới hạn connection được cấu hình trong lab.
4. Ghi ít nhất 2 hypothesis vào `workspace/my-investigation.md`.
5. Sửa `starter/` để resource HTTP được quản lý đúng lifetime.
6. Chạy `verify.ps1` cho tới khi request thứ ba hoàn thành trong budget.
7. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 5.1+ hoặc PowerShell 7+
- Port `5127` còn trống
- Không cần Docker, database hoặc external service

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` build và chạy local Kestrel server cùng HTTP client. Script chỉ pass khi starter thực sự tái hiện được symptom mong đợi.

## Những gì cần quan sát

- status code và header của request 1, 2
- request 3 có bắt đầu nhưng không tiến triển trong budget hay không
- CPU không phải bottleneck chính
- downstream endpoint vẫn đang hoạt động
- relationship giữa số request đang giữ response và `MaxConnectionsPerServer`

Không kết luận root cause chỉ từ việc thấy timeout.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

> **Spoiler:** chỉ mở sau khi bạn đã tự reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Trước khi sửa:

- request 1 và 2 nhận `200` + `X-Document-Id`
- request 3 hết budget khoảng dưới 1 giây mà chưa nhận header
- server không crash

Sau khi sửa:

- cả 3 request hoàn thành trong budget
- không tăng `MaxConnectionsPerServer`
- không retry để che symptom
- resource lifetime rõ ràng và có thể giải thích được

## Estimated Time

35–55 phút.
