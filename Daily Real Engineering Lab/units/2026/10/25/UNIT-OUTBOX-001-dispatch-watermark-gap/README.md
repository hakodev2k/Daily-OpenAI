# UNIT-OUTBOX-001 — Sự kiện bị bỏ qua sau một lần gửi thất bại

## Mục tiêu
Điều tra một incident trong worker phát integration events từ transactional outbox. Reproduce lỗi, thu thập bằng chứng về thứ tự xử lý và checkpoint, đề xuất hypothesis, sửa code và chứng minh rằng sự kiện không bị mất khi dependency trả lỗi tạm thời.

## Bối cảnh thực tế
Hệ thống Loyalty phát sự kiện cộng điểm cho đối tác sau khi xác nhận giao dịch. Outbox chứa các sự kiện có sequence tăng dần. Một worker đơn đọc theo batch và gửi sang message broker. Sau một lỗi 503 ngắn, dashboard của đối tác thiếu một sự kiện, dù worker vẫn báo batch hoàn tất và các sự kiện mới hơn đã đến nơi.

Bài lab mô phỏng outbox và broker **in-memory**, có fault injection xác định. Không cần Azure, Docker, SQL Server hoặc broker thật. Phạm vi giả định **một dispatcher active**, thứ tự sequence là thứ tự phát sự kiện.

## Bạn cần làm gì
1. Chạy incident reproduction, ghi lại các attempt, delivery và checkpoint.
2. Viết ít nhất hai hypothesis và một phép kiểm chứng để loại trừ từng hypothesis.
3. Chỉnh duy nhất `starter/OutboxPipeline.cs`; không chỉnh acceptance harness.
4. Chạy verification cho failure-path, happy-path, batch boundary và replay sau lỗi ghi checkpoint.
5. Đọc reference solution sau khi đã thử sửa và ghi lại trade-off giữa at-least-once và exactly-once.

## Yêu cầu môi trường
.NET 8 SDK (SDK 8.0.100 hoặc feature band mới hơn trong .NET 8); PowerShell 7 hoặc Windows PowerShell 5.1. Không có NuGet package ngoài framework.

## Chạy nhanh
Từ thư mục unit:

```powershell
.\scripts\setup.ps1
.\scripts\run.ps1
.\scripts\reproduce.ps1
```

## Cách reproduce vấn đề
```powershell
.\scripts\reproduce.ps1
```

Exit code **0** ở trạng thái starter nghĩa là **incident được tái hiện**, không phải hệ thống đúng. Quan sát hai lượt dispatch liên tiếp và một lỗi transient được inject đúng một lần.

## Những gì cần quan sát
- Các sequence được thử gửi và được downstream chấp nhận ở mỗi lượt.
- Checkpoint sau mỗi batch và tập các sequence được trả về trong batch kế tiếp.
- Có sự kiện nào không xuất hiện trong danh sách downstream đã nhận dù event đã tồn tại trong outbox không?
- Một lần phát thành công nhưng lưu checkpoint thất bại tạo ra loại rủi ro nào khác?

Dùng [incident timeline](evidence/incident-timeline.md) nếu cần đối chiếu dữ liệu; không mở solution trước khi có hypothesis.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong [workspace](workspace/my-investigation.md).
3. Thử fix trong `starter/OutboxPipeline.cs`.
4. Verify bằng `.\scripts\verify.ps1` (**phải exit 0**).
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ mở sau khi tự điều tra:** [Phân tích và code tham chiếu](solution/README.md).

## Expected Results
[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md)

## Estimated Time
60–75 phút (L3 Production Incident, D4).
