# Before — Starter

Chạy `pwsh -File ./reproduce.ps1`: hai request cùng ETag đều nhận HTTP-like 200; revision cuối là 8, chỉ một ghi chú còn lại. Exit code 0 nghĩa là **đã tái hiện bug**, không phải đã sửa xong.

Chạy `pwsh -File ./verify.ps1` trên starter chưa sửa phải thất bại (exit code 1) vì hai request không được phép cùng thành công. Nếu gặp timeout ở rendezvous, kiểm tra cả hai request có được gọi và không sửa harness.
