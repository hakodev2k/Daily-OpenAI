# After — Learner Fix

Chạy `pwsh -File ./verify.ps1` trên **starter đã sửa**: một request nhận 200 và ETag mới `"8"`, request còn lại nhận 412; revision cuối 8, ghi chú lưu khớp request thành công. Thiếu If-Match trả 428; tag không hợp lệ trả 400; stale tag trả 412. Không có request bị từ chối nào thay đổi ghi chú. Exit code 0 nghĩa là kiểm thử hành vi đã pass.

Tùy scheduler, A hoặc B có thể thắng; bài kiểm tra không phụ thuộc vào thứ tự thắng.
