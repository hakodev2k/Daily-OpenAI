# After — starter đã sửa

- **./verify.ps1** trả exit code 0 và in **Verification: 6/6 passed.**
- Mixed batch có North thành công 42 rows, West lỗi TimeoutException và East lỗi InvalidOperationException.
- Một lỗi không xóa kết quả thành công của nguồn khác.
- Cancellation ném OperationCanceledException, không biến thành lỗi nguồn.
- Ngoại lệ được ném đồng bộ khi gọi adapter được quy về đúng nguồn, không chặn việc chạy các nguồn khác.
- Batch rỗng trả báo cáo rỗng hợp lệ.
- **./reproduce.ps1** không còn tái hiện triệu chứng của starter gốc và có thể trả exit code 1; đây là hành vi đúng sau khi sửa.
