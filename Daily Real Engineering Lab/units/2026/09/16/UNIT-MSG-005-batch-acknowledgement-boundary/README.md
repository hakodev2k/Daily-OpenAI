# UNIT-MSG-005 — Một message lỗi làm cả batch bị xử lý lại

## Mục tiêu
Điều tra một message consumer xử lý theo batch khi hệ thống xuất hiện duplicate side effects dù phần lớn message đã xử lý thành công.

## Bối cảnh thực tế
Một worker nhận các event cập nhật trạng thái đơn hàng theo batch. Bình thường throughput tốt, nhưng khi một event malformed xuất hiện giữa batch, monitoring cho thấy nhiều event hợp lệ trước đó lại được xử lý thêm lần nữa ở lần nhận kế tiếp. Business bắt đầu thấy email trạng thái bị gửi lặp.

## Bạn cần làm gì
1. Chạy starter và reproduce incident.
2. Ghi lại event nào đã tạo side effect và event nào xuất hiện lại ở lần delivery sau.
3. Đưa ra ít nhất 3 hypothesis về boundary giữa xử lý thành công, failure và acknowledgement.
4. Sửa `starter/` để một item lỗi không khiến các item đã hoàn tất tạo side effect lần hai.
5. Chạy `verify.ps1` để kiểm tra behavior sau khi sửa.
6. Chỉ xem reference solution sau khi đã thử điều tra.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell 7+ khuyến nghị

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script chạy một broker simulator deterministic với batch gồm các event hợp lệ và một event lỗi, sau đó mô phỏng delivery tiếp theo theo trạng thái acknowledgement mà consumer để lại.

## Những gì cần quan sát
- Danh sách message được delivery ở từng attempt.
- Side-effect count theo `OrderId`.
- Message nào được broker xem là hoàn tất sau mỗi attempt.
- Behavior phải deterministic, không phụ thuộc timing máy.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler — chỉ xem sau khi đã tự thử](solution/README.md)

## Expected Results
**Before:** một failure trong batch khiến ít nhất một message đã tạo side effect ở attempt đầu lại tạo side effect ở attempt sau.

**After:** message hợp lệ đã hoàn tất không tạo side effect lần hai; message lỗi vẫn được giữ lại để có thể xử lý theo failure policy.

## Estimated Time
45–70 phút.