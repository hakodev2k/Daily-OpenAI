# Investigation guide (không chứa đáp án)

## Incident timeline
Chạy `./reproduce.ps1`, ghi lại 4 sự kiện của harness. Không đoán từ tên method; quan sát giá trị `host-startup-before-release`.

## Competing hypotheses
- Host đang chờ một dependency registration hoặc configuration?
- Worker có làm một bước dài trong startup lifecycle?
- Có phải host startup hoàn tất nhưng readiness chưa đúng?

Đối chiếu với `starter/Program.cs`, `ShelterFeedWarmupService.cs` và API lifecycle của `BackgroundService`.

## Thí nghiệm
- Gate đóng cho tới khi harness chủ động mở.
- Thử thay đổi thứ tự công việc trong worker nhưng **không xóa warmup**.
- `reproduce.ps1` chứng minh starter lỗi; `verify.ps1` phải chạy trên source mà bạn vừa sửa.
- Nếu verification fail do timeout, kiểm tra SDK, mức tải máy và log trước khi tăng timeout.

## Acceptance
Startup có thể hoàn tất trong lúc gate đóng; sau khi mở gate, worker phải hoàn thành và host phải dừng sạch. Đừng đánh đồng việc API nhận request với dữ liệu đã warmup xong.
