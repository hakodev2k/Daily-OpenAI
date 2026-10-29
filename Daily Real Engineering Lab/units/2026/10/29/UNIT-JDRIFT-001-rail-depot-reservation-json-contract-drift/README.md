# UNIT-JDRIFT-001 — Phiếu giữ linh kiện đường sắt bị ghi nhận thành 0 sau thay đổi JSON

## Mục tiêu

Điều tra một sự cố ingest dữ liệu đối tác bằng .NET 8, phân biệt lỗi dữ liệu nguồn, mapping và nghiệp vụ dựa trên bằng chứng; sửa contract để hệ thống không tiếp nhận sai số lượng mà vẫn giữ tương thích với payload hợp lệ.

## Bối cảnh thực tế

Kho bảo trì đường sắt nhận JSON reservation từ đối tác. Trước đợt tích hợp mới, các phiếu giữ linh kiện có số lượng chính xác. Sau một thay đổi nhỏ ở phía đối tác, một số reservation được ghi nhận là **0 units** dù chứng từ nguồn ghi **7 units**. API vẫn trả thành công, không có exception, khiến số lượng có thể cấp phát bị tính dư. Dữ liệu và logs trong lab đều là **synthetic**.

## Bạn cần làm gì

1. Chạy fixture, tái hiện sai lệch và ghi số liệu quan sát.
2. So sánh hai payload, kết quả deserialize và kỳ vọng nghiệp vụ; đề xuất ít nhất ba hypothesis có cách kiểm chứng.
3. Chỉ chỉnh `starter/ReservationReader.cs`. Không sửa `checks/` hoặc dữ liệu fixture để làm tests xanh.
4. Định nghĩa rõ hợp đồng v1, cách xử lý field thiếu/không nhận diện, version không hỗ trợ và số lượng không hợp lệ.
5. Chạy `verify.ps1`, ghi lại bằng chứng trước/sau và đề xuất kế hoạch rollout an toàn.

## Yêu cầu môi trường

- .NET SDK 8+ (target `net8.0`); Windows PowerShell 5.1+ hoặc PowerShell 7+.
- Không cần database, Docker, cloud, NuGet package ngoài SDK hay kết nối mạng khi đã có SDK.
- Chạy các lệnh từ thư mục unit.

## Chạy nhanh

```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

Nếu ExecutionPolicy chặn script, dùng `dotnet run --project ./checks/Checks.csproj -- --observe` và `dotnet run --project ./checks/Checks.csproj -- --reproduce`.

## Cách reproduce vấn đề

Trên starter nguyên bản, `./reproduce.ps1` phải trả exit code **0 khi đã chứng minh triệu chứng**; đây không phải tín hiệu đã sửa đúng. So sánh kết quả với [synthetic incident evidence](evidence/incident.md). Sau khi tự sửa, dùng **`./verify.ps1`** để xác nhận code learner.

## Những gì cần quan sát

- Một payload hợp lệ vẫn được đọc đúng.
- Một payload có ý nghĩa nghiệp vụ tương đương nhưng cách biểu diễn khác lại dẫn đến giá trị không mong đợi.
- Một số trường hợp đầu vào không đầy đủ vẫn đi qua mà không phát tín hiệu lỗi.
- Verification yêu cầu chấp nhận đúng contract v1 và từ chối các payload không đáp ứng contract; không được chỉ kiểm tra process khởi động.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trong `starter/`.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler:** [Giải thích và trade-offs](solution/README.md) · [Reference code](solution/ReservationReader.cs). Chỉ mở sau khi đã điều tra và thử sửa.

## Expected Results

[Trước sửa](expected-results/before.md) · [Sau sửa](expected-results/after.md)

## Estimated Time

60–70 phút · L3 Production Incident · D4.
