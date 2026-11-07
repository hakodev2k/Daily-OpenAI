# After
- Ghi A hợp lệ: 200, ETag thay đổi.
- Ghi B bằng validator cũ: 412, dữ liệu A không thay đổi.
- Thiếu If-Match: 428; weak ETag không khớp: 412.
- B đọc ETag mới rồi ghi: 200, revision tiếp tục tăng.
- ./verify.ps1 trên starter/ in CONTRACT_PASS và exit 0.
