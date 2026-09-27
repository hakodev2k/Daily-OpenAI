# Before

- Historical JSON deserialize thành công.
- Raw payload chứa `Status: 1`.
- Starter diễn giải trạng thái khác với business meaning mà fixture lịch sử đại diện.
- Process kết thúc với exit code khác 0 và in `CONTRACT_MISMATCH`.

Nếu không reproduce được, xác nhận đang chạy `starter/SerializationLab.csproj` bằng .NET 8.