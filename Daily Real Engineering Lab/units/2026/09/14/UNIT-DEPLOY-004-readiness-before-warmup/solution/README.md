# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## Symptoms

Ngay sau startup, liveness và readiness đều trả success nhưng `/catalog/count` vẫn trả `503` trong lúc warmup chạy.

## Evidence

Ba HTTP status trong cùng một khoảng thời gian cho thấy health signal không nhất quán với khả năng phục vụ request nghiệp vụ.

## Root cause

Readiness endpoint chỉ phản ánh việc process đã start, không phản ánh state của warmup bắt buộc trước khi instance có thể xử lý catalog request.

## Why the fix works

Liveness tiếp tục trả success khi process hoạt động. Readiness đọc `WarmupState.IsReady`, vì vậy orchestration layer chỉ nên route traffic sau khi warmup hoàn tất.

## How to verify

Chạy `./verify.ps1`. Trước warmup, readiness phải non-200 trong khi liveness vẫn 200. Sau warmup, readiness và business endpoint đều 200.

## Alternative fixes

Có thể dùng ASP.NET Core Health Checks với readiness tag và custom `IHealthCheck` thay vì endpoint thủ công. Trong hệ thống thật, hãy kiểm tra đúng prerequisite cần thiết thay vì mọi dependency có thể tồn tại.

## Wrong / tempting fixes

- Tăng startup delay cố định ở deployment: che triệu chứng và phụ thuộc timing.
- Biến liveness thành dependency check: có thể gây restart loop khi dependency ngoài gặp sự cố.
- Retry request nghiệp vụ từ client như giải pháp chính: không sửa contract routing của instance.

## Production implications

Readiness sai có thể gây lỗi ngắn hạn mỗi rollout, làm giảm success rate và kích hoạt retry không cần thiết. Readiness quá chặt cũng có thể loại instance khỏi traffic vì dependency phụ.

## Trade-offs

Readiness phải đại diện cho khả năng phục vụ request quan trọng nhưng vẫn tránh coupling quá mức với dependency không thiết yếu.

## What a Senior engineer should notice

Health endpoints là operational contract. Liveness trả lời “có nên restart process không?”, còn readiness trả lời “có nên route traffic vào instance không?”. Hai câu hỏi có failure semantics khác nhau.
