# Investigation guide

1. Định nghĩa invariant: output file phải nằm trong vùng nhập được cấp quyền.
2. Ghi absolute path của root, file hợp lệ và file bất thường.
3. Đưa ra ba hypotheses (entry path, permissions, retry/cleanup) và một phép thử bác bỏ mỗi hypothesis.
4. Chạy `./reproduce.ps1` trên starter; không coi exit 0 là đã sửa lỗi.
5. Thay đổi duy nhất `starter/ArchiveImportService.cs`, chạy `./verify.ps1`.
6. Kiểm tra khi entry hợp lệ đứng trước entry không hợp lệ: partial write có phù hợp hợp đồng nhập liệu không?
7. Ghi kết quả vào `workspace/my-investigation.md`.
