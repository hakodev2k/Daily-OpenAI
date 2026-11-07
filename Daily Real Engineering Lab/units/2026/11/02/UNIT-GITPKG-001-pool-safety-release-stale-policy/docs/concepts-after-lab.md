# Sau lab: Git provenance và artifact integrity

Một commit định nghĩa snapshot nội dung versioned; working tree có thể chứa tracked, modified, untracked và ignored files. Lệnh checkout không phải là cơ chế dọn sạch mọi file ngoài revision. Nếu packaging quét toàn bộ thư mục, artifact có thể phụ thuộc trạng thái của runner.

Để đảm bảo reproducible release: xác định allowlist có version, kiểm tra input, tạo manifest từ chính tập file được đóng gói, chạy negative tests với file ngoài allowlist, lưu artifact immutable và promotion cùng một digest qua môi trường. Khi generated assets là bắt buộc, quản lý provenance và checksum riêng thay vì chỉ sử dụng `git ls-files`.

Cần phân biệt `git ls-files` (index) với `git ls-tree` (commit tree). Trong pipeline thực tế có thể dùng commit tree tại pinned SHA và kiểm tra working tree sạch, nhằm tránh các tracked file đã bị sửa mà chưa commit.
