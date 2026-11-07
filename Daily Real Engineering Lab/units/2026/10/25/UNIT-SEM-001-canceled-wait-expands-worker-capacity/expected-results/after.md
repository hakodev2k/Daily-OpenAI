# After — learner đã sửa
- `./scripts/verify.ps1` in `A still running when C entered: False`, `B cancellation observed: True`, `Renderer exception did not poison gate: True`, `VERIFIED`; exit 0.
- `./scripts/reproduce.ps1` exit 1 (sự cố không còn tái hiện).
- C chỉ vào sau khi A hoàn tất; lỗi renderer không khiến các job sau kẹt.
- Không dựa vào số milliseconds; kiểm tra thứ tự sự kiện và contract.
- Nếu verify thất bại: kiểm tra cả đường đi hủy **trước** khi acquire và exception **sau** khi acquire.
