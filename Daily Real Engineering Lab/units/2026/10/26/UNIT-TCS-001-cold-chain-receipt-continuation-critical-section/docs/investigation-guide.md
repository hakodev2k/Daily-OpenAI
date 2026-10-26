# Investigation guide — chưa tiết lộ lời giải

## Evidence cần thu thập
1. Ghi stdout của `./reproduce.ps1` và `./verify.ps1` trên starter chưa sửa.
2. Vẽ timeline cho một pallet: `Track` → `WaitAndAuditAsync` → `Accept` → audit → task completed.
3. Tại mỗi bước, ghi thread ID, `Monitor.IsEntered`, và elapsed time.
4. Đưa ra ít nhất hai nguyên nhân khả dĩ và một thí nghiệm để bác bỏ từng nguyên nhân.

## Giới hạn thay đổi
Chỉ sửa `starter/ReceiptCoordinator.cs`. Giữ public API, dữ liệu receipt, và số audit. Không chỉnh `Program.cs`, `verify.ps1`, `reproduce.ps1` để làm test pass.

## Mở rộng khi đã pass
Thử nhiều pallet và concurrent acknowledgements, thêm timeout cho pending receipt, kiểm tra duplicate và cancellation. Đừng suy luận throughput chỉ từ một lần đo.
