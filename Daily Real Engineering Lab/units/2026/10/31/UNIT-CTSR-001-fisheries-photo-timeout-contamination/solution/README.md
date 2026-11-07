# Reference Solution — spoiler

**1. Symptoms:** Ảnh đầu timeout hợp lý, hai ảnh nhanh cũng bị timeout; caller cancellation phải propagate.

**2. Evidence:** Cùng một `PhotoInspectionService` xử lý liên tiếp; simulator của ảnh 2/3 trả `CompletedTask` nhưng gọi `ThrowIfCancellationRequested` trước.

**3. Root cause:** Field `CancellationTokenSource _deadline` tồn tại xuyên nhiều operation. Sau ảnh đầu, source đã canceled. `CancelAfter` không reset trạng thái canceled, nên token liên kết của các ảnh sau cũng canceled.

**4. Why fix works:** Dùng một linked `CancellationTokenSource` mới trong từng `InspectAsync`, đặt deadline cho đúng operation, dispose sau khi kết thúc. Khi bắt `OperationCanceledException`, chỉ trả `TimedOut` nếu deadline operation bị hủy và caller chưa hủy.

**5. How to verify:** Copy [PhotoInspectionService.cs](PhotoInspectionService.cs) vào `starter/PhotoInspectionService.cs` chỉ sau khi tự thử, chạy `./verify.ps1`. Test kiểm tra slow → fast → fast và pre-cancelled caller trên cùng service.

**6. Alternative:** Tạo timeout source và linked source riêng từng lần gọi, dispose cả hai. Hoặc dùng timeout do downstream cung cấp nếu contract rõ.

**7. Wrong fixes:** Tăng timeout không reset source; `Task.Run` không sửa ownership; luôn bắt cancellation thành timeout sẽ nuốt caller cancellation; tạo service mới mỗi ảnh có thể giấu lỗi lifecycle.

**8. Production implications:** Ghi correlation ID, phân loại caller cancel vs deadline; timeout của operation có side effect không chứng minh downstream chưa commit.

**9. Trade-offs:** Source per-operation có allocation/callback, nhưng ownership rõ và isolation. Benchmark trước khi tối ưu hot path.

**10. Senior observation:** Cần regression chuỗi nhiều operation; test một ảnh không phát hiện trạng thái bị nhiễm. Không tự động coi timeout là kết quả chắc chắn.

**Validation:** Static review only; .NET code chưa được build/run trong lần tạo lab.
