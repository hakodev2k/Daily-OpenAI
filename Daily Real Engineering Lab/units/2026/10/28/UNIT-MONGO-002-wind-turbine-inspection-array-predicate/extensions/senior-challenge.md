# Senior challenge — giữ đúng khi mở rộng

1. Product yêu cầu chọn turbine có `brake=FAILED` **và** inspection phanh thất bại gần nhất chưa được xử lý. Đề xuất schema và query khi mỗi turbine có hàng nghìn inspection.
2. Thiết kế idempotent ticket creation khi job bị chạy lại; giữ business key nào để không tạo ticket trùng?
3. Đề xuất 2 metrics và 1 canary regression test giúp phát hiện false-positive routing sau deploy.
4. Đánh giá khi nào nên tách inspection thành collection riêng, với trade-offs write amplification, indexing, retention và operational cost.
