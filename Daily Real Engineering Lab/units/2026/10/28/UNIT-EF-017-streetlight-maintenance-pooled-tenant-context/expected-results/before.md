# Before — starter chưa sửa

Với fixture mặc định, request north đầu tiên pass, request south và new-tenant trả N-101 sai isolation contract, request north cuối cùng pass. Harness kết thúc bằng **ISOLATION_CHECK failures=2 total=4** và exit code **17**. Script reproduce xác nhận baseline khi nhận đúng exit code này.

Nếu không thấy triệu chứng, kiểm tra SDK .NET 8, NuGet restore, SQLite in-memory connection còn mở, poolSize=1 và thứ tự fixture. Build error không phải bằng chứng bug đã được reproduce.
