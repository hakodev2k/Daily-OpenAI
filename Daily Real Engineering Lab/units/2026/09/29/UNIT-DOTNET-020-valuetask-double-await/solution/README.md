# Reference Solution

Starter giữ một `ValueTask<T>` rồi consume hai lần. Fast path chứa result trực tiếp nên có vẻ đúng; slow path backed bởi source có single-consumption contract và fail ở lần thứ hai.

Fix nhỏ nhất là await operation một lần, lưu result value rồi reuse value cho calculation và telemetry.

Nếu nhiều consumers thật sự cần chờ cùng operation, có thể materialize một `Task<T>` có chủ đích bằng `AsTask()`, với allocation/semantics trade-off.

Wrong fixes: ép dependency luôn synchronous; nuốt exception telemetry; gọi dependency lần hai; hoặc đổi API rộng hơn mà không xét contract và allocation.

Senior takeaway: không suy ra async contract từ fast-path behavior. Consumer phải tôn trọng consumption contract của `ValueTask<T>`.
