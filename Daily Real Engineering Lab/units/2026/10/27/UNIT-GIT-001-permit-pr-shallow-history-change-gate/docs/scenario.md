# Incident brief — Permit PR change gate

- Backend: .NET permit policy service; CI chọn targeted tests dựa trên file thay đổi.
- Một PR giả lập đổi ngưỡng MaxFloors và thêm tài liệu nghiệp vụ.
- CI checkout ở thư mục .lab-runtime/shallow; remote origin.git hoàn toàn local.
- Build và unit tests vẫn có thể đọc source hiện tại; riêng change gate lỗi.
- Đầu vào hợp đồng: WorkingCopy và BaseSha do hệ thống CI cung cấp.
- Output hợp đồng: danh sách path tương đối từ repository root, mỗi dòng một file; nonzero exit nếu không thể xác minh base.
- Không được trả danh sách rỗng như một thành công khi thiếu dữ liệu so sánh.
- Không được tự ý dùng trạng thái nhánh main hiện tại thay cho BaseSha nếu lịch sử đã thay đổi.

Điều tra bằng git log, git rev-parse, git cat-file, git branch -a, git remote -v. Đối chiếu cả local seed và checkout của runner. Không sửa setup.ps1 để làm bài test dễ hơn.
