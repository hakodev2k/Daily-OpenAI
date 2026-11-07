# Incident notes — bằng chứng giả lập

09:00: A/B/C được nhập và tra cứu chính xác.

09:05: Writer ghi A v2, B v2, xóa C, thêm D. Chỉ có notification của B được chuyển tới indexer. Không có lỗi I/O.

09:06: Health loop ghi maintenanceTicks=2; CPU thấp, không exception. Kết quả tìm kiếm chưa khớp thư mục.

09:10: B bị xóa có notification; D đổi trạng thái, E được thêm nhưng không có notification trong mô phỏng. Sau tick thứ ba, B biến mất khỏi index, sai lệch khác vẫn còn.

Notification được cố ý bỏ qua trong harness để tái hiện xác định. Đây không phải phép đo tỷ lệ mất event của FileSystemWatcher trên hệ điều hành thực.
