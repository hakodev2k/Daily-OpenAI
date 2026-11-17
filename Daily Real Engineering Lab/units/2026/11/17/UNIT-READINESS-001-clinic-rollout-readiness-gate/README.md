# UNIT-READINESS-001 — Clinic rollout: readiness gate under cold start

## Mục tiêu
Điều tra một incident rollout, tách tín hiệu health khỏi traffic readiness và xác minh một bản sửa bằng replay có thể lặp lại.

## Bối cảnh thực tế
Hệ thống đặt lịch khám triển khai phiên bản mới. Một số request đầu tiên nhận 503 trong khi dashboard ghi nhận container đã khởi động. Sau vài giây hệ thống tự hồi phục. Sự cố tăng khi rolling deployment.

## Bạn cần làm gì
Chạy mô phỏng, ghi nhận timeline và các request bị từ chối; đưa ra ít nhất hai hypothesis; sửa chính sách nhận traffic trong `starter/Program.cs` mà không làm chậm việc phát hiện process chết.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell 7+ hoặc Windows PowerShell 5.1. Không cần Docker, Azure hay NuGet bên ngoài.

## Chạy nhanh
Từ thư mục unit: `./scripts/setup.ps1`, sau đó `./scripts/reproduce.ps1`.

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1`. Mô phỏng 8 giây đầu của một pod và request mỗi giây.

## Những gì cần quan sát
Ghi lại giây đầu tiên traffic được chuyển vào, giây đầu tiên service trả 200, tổng số 503, và tín hiệu liveness. Đối chiếu `evidence/incident.md`.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ xem sau khi tự điều tra: [Solution](solution/README.md).

## Expected Results
Trước: có request 503 trong rollout; sau: không gửi traffic đến instance chưa phục vụ được, vẫn giữ tín hiệu liveness độc lập.

## Estimated Time
50–70 phút.
