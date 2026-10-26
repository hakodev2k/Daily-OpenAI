# After — starter đã sửa
- `./verify.ps1`: **9/9 PASS**, exit code **0**.
- Poll không đổi: 1 lần apply. Revision hoặc ngưỡng thay đổi: 2 lần apply. Hai zone độc lập: 2 lần apply.
- Dữ liệu nguồn bị mutate sau khi apply không được âm thầm thay đổi snapshot đã lưu.
- `./reproduce.ps1` chỉ dùng để chứng minh lỗi ban đầu; khi lỗi đã được sửa, nó có thể trả exit code 1 vì symptom không còn.
