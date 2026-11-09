# Senior challenge

Giả sử 10% trạm chỉ có zone được xác định theo lịch sử thời gian, không theo station_id hiện tại. Thiết kế bảng mapping có effective_from/effective_to, cách xử lý overlapping intervals, reconciliation và audit trail. Giải thích vì sao backfill không được dùng current zone cho sự kiện lịch sử.