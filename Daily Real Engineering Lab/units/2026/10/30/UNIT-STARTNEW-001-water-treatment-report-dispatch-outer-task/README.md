# UNIT-STARTNEW-001 — Báo cáo chất lượng nước được đánh dấu gửi thành công trước khi đối tác xác nhận

## Mục tiêu

Điều tra tính đúng đắn của vòng đời một tác vụ gửi báo cáo bất đồng bộ: đo thời điểm job hoàn tất, thu thập bằng chứng về phản hồi đối tác, đề xuất bản sửa và kiểm chứng đường lỗi.

## Bối cảnh thực tế

Hệ thống nhà máy xử lý nước tạo báo cáo kiểm nghiệm hằng ngày và chuyển đến cổng quản lý. Dashboard đôi khi hiển thị **SENT** ngay cả khi cổng chưa phản hồi; khi đối tác từ chối, job vẫn được ghi nhận là thành công. Nhóm vận hành không thấy exception ở ranh giới job.

## Bạn cần làm gì

1. Chạy baseline, ghi lại hai kết quả kiểm tra bị lỗi và một trường hợp thành công.
2. Xác định thời điểm job trả về so với thời điểm gateway được giải phóng.
3. Viết ít nhất hai hypothesis và phân biệt lỗi timeout, lỗi gateway và lỗi điều phối.
4. Chỉ sửa `starter/ReportDispatcher.cs`, giữ nguyên `IReportGateway` và không thay đổi `harness/`.
5. Chạy verify và giải thích hậu quả với retry, acknowledgement và observability trong production.

## Yêu cầu môi trường

Windows PowerShell 5.1+ hoặc PowerShell 7; .NET 8 SDK/runtime. Không cần Docker, database, cloud hay package ngoài .NET SDK.

## Chạy nhanh

Tại thư mục unit:

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script trả mã 0 chỉ khi phát hiện đúng baseline failure; lỗi build hoặc thiếu SDK không được xem là reproduce thành công.

## Những gì cần quan sát

- `CASE=WAITS_FOR_SEND`: trạng thái job khi gateway vẫn đang giữ phản hồi.
- `CASE=PROPAGATES_FAILURE`: kết quả job khi đối tác trả lỗi.
- `CASE=SUCCESS_PATH`: số lần gửi và kết quả khi đối tác chấp nhận.
- Chênh lệch giữa một `Task` được chờ và toàn bộ hoạt động mà job sở hữu.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis trong [workspace/my-investigation.md](workspace/my-investigation.md).
3. Thử fix.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md).

## Reference Solution

**Spoiler:** [solution/README.md](solution/README.md) — chỉ xem sau khi tự điều tra.

## Expected Results

[Before](expected-results/before.md) · [After](expected-results/after.md). `verify.ps1` kiểm tra **starter do người học sửa**, không kiểm tra riêng solution.

## Estimated Time

40–50 phút (L2 / D4).
