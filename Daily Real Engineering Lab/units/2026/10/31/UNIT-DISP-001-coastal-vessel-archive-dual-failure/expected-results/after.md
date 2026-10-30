# After — starter do học viên sửa

`./verify.ps1`: exit 0, **4/4 PASS**.

- `both`: `AggregateException` chứa đúng hai inner exceptions theo thứ tự `ArchiveUploadException`, `ArchiveFinalizeException`.
- `upload-only`: giữ `ArchiveUploadException`.
- `finalize-only`: giữ `ArchiveFinalizeException`.
- `success`: không có exception.
- Mọi tình huống: upload đúng một lần, finalization đúng một lần.

`./reproduce.ps1` không còn xác nhận triệu chứng gốc sau khi sửa (exit khác 0); đó là chủ đích, không phải lỗi verify.
