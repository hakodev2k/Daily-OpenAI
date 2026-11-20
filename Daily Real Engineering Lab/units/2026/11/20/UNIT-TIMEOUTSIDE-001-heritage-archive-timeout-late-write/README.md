# UNIT-TIMEOUTSIDE-001 — Timeout trả về nhưng dữ liệu vẫn thay đổi

## Mục tiêu
Điều tra giới hạn thời gian xử lý bất đồng bộ, quan sát trạng thái sau khi client nhận timeout, thiết kế fix không làm mất tính đúng đắn.

## Bối cảnh thực tế
Hệ thống số hóa tư liệu nhận yêu cầu nhập bản ghi từ đối tác. Một số yêu cầu trả TIMEOUT sau 80 ms, nhưng bảng kiểm kê thay đổi muộn; retry có thể tạo kết quả không mong muốn.

## Bạn cần làm gì
Reproduce; ghi lại timeline, trạng thái kho ngay khi timeout và sau 300 ms; đưa ra hai giả thuyết; sửa code trong `starter/Program.cs` và chứng minh không còn late write; giải thích điểm commit và idempotency nếu client retry.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7+ hoặc Windows PowerShell 5.1; không cần database/network.

## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`; ghi lại TIMEOUT, count tại thời điểm phản hồi và count sau khoảng đợi.

## Những gì cần quan sát
Đối chiếu timestamp của phản hồi với lúc side effect xuất hiện; kiểm tra xem yêu cầu bị từ chối có làm thay đổi dữ liệu hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải](solution/README.md).

## Expected Results
Starter: TIMEOUT và xuất hiện ghi muộn. Sau fix: TIMEOUT và không có side effect muộn; yêu cầu nhanh vẫn ghi đúng một lần.

## Estimated Time
40 phút.
