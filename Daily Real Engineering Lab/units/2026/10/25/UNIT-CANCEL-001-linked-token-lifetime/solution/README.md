# Reference Solution — chỉ đọc sau khi đã reproduce và tự sửa

## Symptoms
Khi caller hủy request hoặc host bắt đầu shutdown, scanner tiếp tục pending dù token ở boundary đã bị cancel. Không có cancellation, receipt vẫn thành công.

## Evidence
Probe tạo một `ControlledScanner` chờ `TaskCompletionSource`. Sau khi service trả về task, probe cancel riêng caller hoặc host. Starter ghi `downstreamTokenCancelled=False`, `taskCancelled=False`, `pendingBeforeRelease=True`. Probe chủ động release scanner sau khi đo để không để lại orphan task. Đây là mô phỏng local, không phải production trace thật.

## Root cause
`DocumentValidationService.ValidateAsync` trả thẳng `Task<ScanReceipt>` từ scanner. `using var linked` bị dispose ngay khi method **trả về task**, không phải khi task hoàn thành. Dispose linked `CancellationTokenSource` hủy đăng ký chuyển tiếp từ hai nguồn token; hủy caller/host về sau không làm linked token bị cancel. Scanner vẫn chờ.

## Why the fix works
Thay method bằng `async Task<ScanReceipt>` và `await _scanner.ScanAsync(request, linked.Token)` **bên trong** scope `using`. Compiler giữ scope qua `await` cho tới khi scanner hoàn tất hoặc bị hủy, rồi mới dispose linked source. `ConfigureAwait(false)` không phải cơ chế sửa lỗi; nó chỉ thể hiện library-style await.

Xem [DocumentValidationService.cs](DocumentValidationService.cs).

## Verification
1. Starter nguyên bản: `reproduce.ps1` phải in `EXPECTED_REPRODUCED`.
2. Tự sửa starter rồi chạy `verify.ps1`; cần `VERIFIED`.
3. Reference độc lập: `verify.ps1 -Reference`; không thay cho kiểm thử starter.
4. Probe kiểm tra caller cancellation, host cancellation và normal completion. Không được chỉ kiểm tra token nguồn đã cancel.

## Alternative fixes
- Một wrapper async riêng sở hữu linked CTS cho toàn bộ operation: hợp lý nếu muốn tách orchestration khỏi public API.
- Nếu hợp đồng chỉ cần một token, có thể truyền trực tiếp token đó, nhưng **không** đáp ứng yêu cầu kết hợp caller và host của bài này.
- Dùng `try/finally` trong một async method để dispose sau khi await cũng hợp lệ; `using` đã biểu đạt chính xác cùng ý định.

## Wrong / tempting fixes
- Tăng HTTP timeout hoặc `Task.Delay`: chỉ che triệu chứng; không khôi phục propagation.
- Trả `_scanner.ScanAsync(request, linked.Token)` từ trong `using` mà không await: vẫn dispose sớm.
- Tạo linked CTS rồi không bao giờ dispose: có thể giữ registrations và tài nguyên quá lâu.
- Chỉ truyền `requestAborted`: bỏ mất host shutdown.
- Gọi `Cancel()` trong `finally` khi method vừa trả task: hủy nhầm cả scan hợp lệ.

## Production implications
Với ASP.NET Core, truyền `HttpContext.RequestAborted` từ request boundary và `IHostApplicationLifetime.ApplicationStopping` theo hợp đồng vận hành. Downstream client phải thực sự tôn trọng cancellation token. Bảo đảm cleanup và timeout budget phù hợp khi partner không phản hồi; cancellation là cooperative, không cưỡng bức kill I/O tùy ý.

## Trade-offs
Linked CTS tạo registrations và có chi phí nhỏ; dùng khi cần kết hợp nhiều cancellation domains. Một async wrapper làm rõ ownership nhưng thêm state machine không đáng kể so với outbound I/O. Nếu thao tác cần tiếp tục độc lập sau khi request kết thúc (durable background job), kiến trúc phải tách job lifecycle khỏi request thay vì dùng workaround để bỏ cancellation.

## Senior engineer should notice
`Task` biểu diễn lifetime của operation, còn `using` trong non-async method chỉ bao lifetime thực thi method. Hãy kiểm tra ownership và thời điểm cleanup ở mọi code trả task chưa await, đặc biệt linked CTS, streams, DI scopes và locks. Một cancellation callback ở request boundary không chứng minh downstream đã dừng.

## Validation mode
`static-review`: mã và scripts đã được rà soát tính nhất quán tĩnh; chưa tuyên bố đã build hoặc chạy trong phiên tạo lab.
