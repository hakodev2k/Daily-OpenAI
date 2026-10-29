# Incident: aquaculture compliance archive

Worker nhập ZIP chứng từ vào intake riêng cho từng submission. Sau một job báo success, review workspace bên cạnh xuất hiện file lạ. Bất biến nghiệp vụ: không entry nào được ghi ngoài intake; nếu ZIP chứa entry vi phạm thì từ chối **toàn bộ gói trước khi ghi bất kỳ entry nào**.

Các giả thuyết cần phân biệt: worker dùng nhầm destination; tác vụ khác tạo file; tên entry có ý nghĩa đặc biệt đối với filesystem; lỗi race/concurrency. Evidence có noise về CPU và retries. Lab tạo sandbox dưới thư mục tạm và tự dọn; không sử dụng thư mục production.

Phạm vi: destination mới, đáng tin, không chứa symlink do actor khác tạo. Hardening production về symlink, quota, zip bomb và atomic publish là bài mở rộng.
