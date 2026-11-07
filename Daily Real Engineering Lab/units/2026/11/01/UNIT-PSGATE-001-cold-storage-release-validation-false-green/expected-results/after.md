# After — hợp đồng sau sửa
Sau khi chỉ sửa `starter/invoke-quality-gate.ps1`:
- `scripts/verify.ps1` trả 0 và in `VERIFY_PASSED`.
- Với `-Mode fail`: validator báo lỗi, wrapper không báo `QUALITY_GATE_PASS`, và process trả nonzero (reference giữ nguyên code 23).
- Với `-Mode pass`: validator và wrapper đều báo pass, process trả 0.
- `scripts/reproduce.ps1` sẽ không còn xác nhận được triệu chứng cũ; đây là kết quả hợp lý sau sửa.
