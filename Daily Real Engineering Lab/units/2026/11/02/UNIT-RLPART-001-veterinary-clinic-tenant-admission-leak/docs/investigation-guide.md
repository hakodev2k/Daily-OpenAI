# Investigation guide

1. Chạy `./reproduce.ps1`; lưu sáu status và exit code.
2. Ghi ít nhất ba hypothesis: admission policy, authentication identity, endpoint grouping, downstream.
3. Đọc `starter/Program.cs` để xác định nguồn danh tính và header nào không đáng tin.
4. Đọc `starter/AdmissionPolicy.cs`; giải thích phạm vi quota thực tế trước khi sửa.
5. Chỉ sửa starter policy, chạy `./verify.ps1`.
6. Đối chiếu invariant: Beta không tiêu quota của Alpha; Alpha không tăng quota khi đổi endpoint; header giả không đổi identity.
7. Phân tích rủi ro khi scale nhiều replica và cardinality của partition key.

Không sửa harness để hardcode kết quả.
