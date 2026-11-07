# After — learner fix

Mỗi tenant chỉ thấy open work order của mình: north → N-101, south → S-202, new-tenant → empty, north → N-101. Harness in **ISOLATION_CHECK failures=0 total=4**, exit code 0; **./verify.ps1** báo VERIFIED.

Kiểm tra thêm: đảo thứ tự tenant; lặp north/south nhiều vòng; không thay đổi dữ liệu seed hay bỏ global filter; kiểm tra request tenant không hợp lệ bị reject. Việc ứng dụng không ném exception chưa đủ để coi là fix.
