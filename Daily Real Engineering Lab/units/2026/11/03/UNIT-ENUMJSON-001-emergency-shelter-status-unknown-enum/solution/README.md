# Reference solution (spoiler)

System.Text.Json có thể nhận numeric value 9 vào enum dù không có member định nghĩa. Nhánh default của routing âm thầm bỏ qua bản ghi. Validate bằng Enum.IsDefined sau Deserialize; ném JsonException với trạng thái lạ, quarantine record nếu cần. Không đổi default thành AVAILABLE; không chỉ log warning. Test 1 và 3 vẫn hoạt động, 9 và chuỗi không hợp lệ bị từ chối. Với batch ingestion, cân nhắc cô lập record sai thay vì bỏ cả batch, bổ sung metric và alert.
