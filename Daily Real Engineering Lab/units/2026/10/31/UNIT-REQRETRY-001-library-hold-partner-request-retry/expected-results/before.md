# Before — starter chưa sửa

- Script reproduce chạy partner giả lập trả 503 rồi chuẩn bị 202.
- Console cho thấy partner nhận đúng một request và client ném InvalidOperationException.
- Script reproduce trả exit code 0 khi phát hiện đúng triệu chứng; đây không phải là xác nhận hệ thống đã được sửa.
- Script verify trả exit code khác 0 vì chưa đạt hợp đồng phục hồi khi 503.
- Nếu không reproduce: xác nhận đang chạy từ unit root, SDK hỗ trợ net8.0 và file starter chưa bị thay đổi.
