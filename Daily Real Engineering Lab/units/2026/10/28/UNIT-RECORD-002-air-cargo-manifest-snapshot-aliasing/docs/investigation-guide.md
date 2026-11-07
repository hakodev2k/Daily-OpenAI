# Investigation Guide

1. Chạy `./reproduce.ps1` trên starter nguyên bản; chụp lại các dòng `first_snapshot_count_after_next_scan`, `reader_could_mutate_snapshot` và `active_manifest_unchanged_by_reader`.
2. Mở `starter/ManifestService.cs`. Đánh dấu các điểm tạo bản chụp, cập nhật trạng thái và trả dữ liệu ra bên ngoài. Ghi giả thuyết về mối liên hệ giữa các object.
3. Thử một thay đổi nhỏ tại starter, chạy `./verify.ps1`; đối chiếu từng assertion in ra.
4. Phân biệt việc sửa ở nơi **Capture** với việc sửa ở nơi **ScanBag**: lựa chọn nào bảo vệ lịch sử, lựa chọn nào bảo vệ trạng thái nội bộ?
5. Xác minh không chỉ count mà cả nội dung bag tags, timestamp và dữ liệu chuyến bay khác.

Không mở solution trước khi ghi evidence và hypothesis.
