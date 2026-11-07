# After — hợp đồng nghiệp vụ đã đúng
- `Stored events for TENANT-ALPHA: 2`
- `Exported rows: 2`
- `IDs: EVT-101,EVT-102` (theo `Id`).
- Không có `EVT-999` của `TENANT-BETA`.
- Không có `Patient` entity nào bị materialize/tracked bởi truy vấn xuất.
- `verify.ps1` exit 0 với `PASS`; `reproduce.ps1` sau khi fix phải không còn reproduce được (exit 1).
- Không yêu cầu so khớp thời gian chạy trên các máy khác nhau.
