# Điều tra theo bằng chứng

1. Chạy baseline ít nhất hai lần, ghi kết quả từng cycle.
2. Xác định chu kỳ thứ hai có thực sự gọi actuator không.
3. Phân biệt timeout của một lần tưới và shutdown của cả worker.
4. Kiểm tra lifetime của các resource có trạng thái giữa hai cycle.
5. Thử sửa nhỏ nhất, chạy verify và ghi trade-off.

## Phân tích sau lab
**Symptoms:** Chu kỳ 1 OK; chu kỳ 2 cancelled dù actuator chỉ mất 25 ms.

**Evidence:** `CancelAfter(120ms)` được đăng ký trước chu kỳ đầu; khoảng nghỉ 160 ms vượt deadline; token cũ đã cancel trước chu kỳ sau.

**Root cause:** CTS budget là field sống xuyên nhiều chu kỳ; một lần cancel là terminal. `CancelAfter` không phục hồi token.

**Why fix works:** Mỗi chu kỳ sở hữu CTS mới và deadline riêng; linked CTS vẫn nhận shutdown.

**Verification:** `pwsh ./scripts/verify.ps1` chạy starter đã sửa. Thử thêm shutdown token bị cancel trong actuator để xác nhận propagation.

**Alternative:** Dùng `CancellationTokenSource.CreateLinkedTokenSource` cho shutdown + `CancelAfter` hoặc tách hai CTS per-cycle.

**Wrong fixes:** Bắt và nuốt mọi `OperationCanceledException` chỉ che lỗi; tăng timeout lên rất lớn làm giảm bảo vệ; `Task.Run` không sửa lifetime.

**Production implications:** Không dùng lại cancelled CTS cho các request/batch khác; dispose resource theo scope; phân biệt cancellation chủ động với timeout trong telemetry.

**Trade-offs:** CTS per-cycle có allocation nhỏ nhưng rõ ownership và tránh cross-cycle state; deadline phải dựa trên SLA actuator.

**Senior signal:** Xác định state ownership và cancellation boundary trước khi tăng timeout hoặc scale worker.
