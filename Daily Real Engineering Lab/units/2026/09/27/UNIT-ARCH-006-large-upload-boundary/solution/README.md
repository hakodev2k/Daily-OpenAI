# Reference Solution — một phương án có thể bảo vệ được

Đây không phải kiến trúc duy nhất đúng. Phương án này tối ưu cho file 2–8 GB, browser clients, backend ASP.NET Core và Azure Blob Storage.

## Decision

Tách control plane khỏi data plane.

Backend vẫn sở hữu:

- authentication/authorization
- tenant và quota checks
- upload session
- metadata
- object naming policy
- trạng thái lifecycle
- finalize command
- publish permission

Nhưng bytes của file lớn không đi xuyên qua ASP.NET Core API. Client upload trực tiếp tới Blob Storage bằng quyền upload scope hẹp, thời gian sống ngắn, chỉ áp dụng cho object/path đã được backend cấp.

## Suggested lifecycle

1. Client gọi create-upload-session.
2. Backend kiểm tra tenant, quota, file metadata dự kiến và tạo upload record ở trạng thái Pending.
3. Backend cấp capability chỉ cho phép upload vào object cụ thể trong khoảng thời gian ngắn.
4. Client upload trực tiếp tới Blob Storage, ưu tiên resumable/chunked behavior phù hợp SDK.
5. Client gọi finalize bằng uploadSessionId.
6. Backend xác minh object tồn tại, size/content properties phù hợp và transition sang Uploaded.
7. Scan pipeline xử lý object.
8. Chỉ khi scan pass và metadata hợp lệ, asset mới chuyển sang Ready/Published.
9. Upload quá hạn hoặc object orphan được cleanup bởi scheduled reconciliation.

## Why not keep the API in the byte path?

Proxy qua API có thể hợp lý với file nhỏ hoặc khi backend bắt buộc phải transform/decrypt/inspect inline. Với file nhiều GB, nó khiến application capacity bị dùng cho long-lived byte transfer, tăng connection occupancy, memory/buffer pressure, scaling cost và retry blast radius.

Tách data plane cho phép object storage xử lý throughput và resumable transfer, trong khi backend giữ business control.

## Security boundary

Capability upload phải:

- chỉ cho phép operation cần thiết
- scope tới object/container path cụ thể
- có expiry ngắn
- không cấp list/read/delete rộng nếu không cần
- gắn với server-side upload record và tenant ownership

Finalize API không được tin client rằng upload "đã xong"; backend phải kiểm tra storage state.

## Retry and idempotency

create-upload-session và finalize cần idempotency rõ ràng.

Finalize nhiều lần cho cùng session phải trả cùng outcome hợp lệ hoặc transition an toàn, không tạo duplicate asset.

Upload retry không được tạo uncontrolled orphan objects. Object key nên gắn với server-generated upload/session identity.

## Antivirus and content validation

Không publish ngay khi upload bytes xong.

Dùng state machine kiểu:

Pending → Uploaded → Scanning → Ready

và failure states riêng như Rejected, Expired, Failed.

Scan có thể async vì file lớn. UI đọc state thay vì giữ HTTP request chờ hàng phút.

## Cleanup

Một reconciliation job định kỳ xử lý:

- Pending quá hạn
- Uploaded nhưng không finalize
- object tồn tại nhưng session không hợp lệ
- session đã fail nhưng blob chưa xóa

Cleanup phải idempotent và có retention window để tránh xóa object đang upload hợp lệ.

## Observability

Theo dõi ít nhất:

- upload sessions created/completed/expired
- bytes transferred
- finalize failures
- scan latency/failure
- orphan cleanup count
- quota rejection
- storage errors
- time từ session creation tới Ready

## Alternative defensible choices

Giữ API proxy vẫn có thể đúng nếu:

- file nhỏ
- compliance bắt buộc inline inspection
- server phải transform bytes trước storage
- client không thể giao tiếp storage trực tiếp
- operational simplicity quan trọng hơn throughput

Một upload gateway chuyên dụng cũng có thể đúng nếu organization cần protocol mediation hoặc centralized ingress controls.

## What a Senior / Architect should notice

Quyết định chính là placement của responsibility.

Object storage nên xử lý bulk data transfer khi có thể; application backend nên giữ business control và security policy. Nhưng tách hai plane tạo thêm lifecycle states, cleanup, idempotency và observability mà design phải giải quyết đầy đủ.
