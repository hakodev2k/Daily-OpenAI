# UNIT-DRAIN-001 — Bản render thẻ hội nghị biến mất khi worker bị dừng

## Mục tiêu
- Tái hiện sự cố job không tạo được thẻ sau khi rolling deployment và một sự cố upload không rõ kết quả.
- Đối chiếu trạng thái queue, trạng thái hoàn tất và số artifact thực tế để đưa ra hypothesis.
- Refactor một ranh giới xử lý job, sau đó chứng minh cả cancellation và lỗi hậu xử lý đều không làm mất việc.

## Bối cảnh thực tế
Dịch vụ tạo thẻ hội nghị nhận các yêu cầu render từ queue. Một đợt triển khai phiên bản mới dừng worker đúng lúc đang render; sau khi worker mới khởi động, một số thẻ không xuất hiện. Ở một ca khác, gateway lưu artifact rồi trả lỗi trước khi gửi receipt. Dashboard không phản ánh đầy đủ hai sự cố.

Bài lab dùng simulator in-memory có thể chạy trên máy cá nhân. Không cần Kubernetes hay broker thật; các thời điểm lỗi được điều khiển để luôn tái hiện.

## Bạn cần làm gì
1. Chạy baseline và ghi lại bảng số liệu của hai case.
2. Đưa ra ít nhất ba hypothesis; phân biệt queue delivery, render và artifact publishing.
3. Chỉ chỉnh `starter/BadgeWorker.cs`; giữ nguyên `Program.cs`, `JobStore.cs` và `BadgeRenderer.cs`.
4. Chạy `./verify.ps1` trên **starter đã sửa**; giải thích tại sao hai loại lỗi cần cùng một hợp đồng xử lý.
5. Nêu thêm hai giới hạn khi chuyển mô phỏng này sang broker thật.

## Yêu cầu môi trường
- .NET SDK 8.x, PowerShell 7 hoặc Windows PowerShell 5.1.
- Không cần NuGet ngoài SDK, Docker, Azure hoặc dữ liệu thật.

## Chạy nhanh
~~~powershell
./setup.ps1
./run.ps1
./reproduce.ps1
~~~

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` trước khi sửa code. Exit code 0 cùng `EXPECTED_FAILURE_REPRODUCED` nghĩa là **đã tái hiện lỗi**, không phải hệ thống đã đúng. Xem thêm [incident timeline](evidence/incident-timeline.md).

## Những gì cần quan sát
- Sau tín hiệu dừng worker, job đang ở trạng thái nào? Artifact đã tồn tại chưa?
- Worker mới còn nhận được công việc nào không?
- Khi upload có side effect nhưng trả exception, trạng thái cuối khác gì?
- Có thể chứng minh một artifact không bị publish trùng trong lần chạy lại không?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis và evidence vào [workspace](workspace/my-investigation.md).
3. Thử fix trên starter.
4. Verify cả cancellation và lỗi sau publish.
5. Chỉ sau đó mới xem reference solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) — chỉ mở sau khi tự điều tra và thử sửa.

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
60–75 phút. Nếu còn thời gian, làm [Senior Challenge](extensions/senior-challenge.md).
