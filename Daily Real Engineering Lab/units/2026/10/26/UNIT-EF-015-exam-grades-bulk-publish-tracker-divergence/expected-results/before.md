# Before — starter

~~~powershell
./reproduce.ps1
# Persisted: Id=41, Score=92, Status=Draft, Note=QA-reviewed
# CONTRACT PASS
./verify.ps1
# CONTRACT FAIL (exit code 1)
~~~

Không có lỗi SQL. Batch báo 1 row affected nhưng trạng thái cuối request vẫn là Draft. Nếu restore thất bại, kiểm tra .NET SDK 8.x, kết nối NuGet và proxy trước khi điều tra nghiệp vụ.
