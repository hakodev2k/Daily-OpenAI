# Before
- `./run.ps1`: exit 0; stdout `Station NorthStation: AQI 42.`.
- `./reproduce.ps1`: exit 0 vì chứng minh được lần chạy ở project folder thành công nhưng ở temp folder thất bại.
- `./verify.ps1`: fail do scheduled launch context.
- Nếu không tái hiện: xác nhận starter chưa bị sửa, có .NET 8 SDK và packaged template nằm trong output.
