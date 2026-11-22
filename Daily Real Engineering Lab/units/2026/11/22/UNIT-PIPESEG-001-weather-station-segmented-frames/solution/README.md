# Reference Solution — chỉ đọc sau khi tự điều tra

## Symptoms
Một payload chứa ba bản ghi nhưng kết quả thay đổi theo cách transport chia buffer. Một segment hoạt động, nhiều segment có thể mất dữ liệu.

## Evidence
`Fixtures.Frame()` và `Fixtures.Frame(5,6,7)` chứa cùng byte, nhưng `ReadOnlySequence<byte>` có cấu trúc segment khác. Kiểm tra `Length` và `FirstSpan.Length`.

## Root cause
Starter dùng `payload.FirstSpan`, chỉ trả về segment đầu tiên, không phải toàn bộ sequence. `Encoding.UTF8.GetString` vì vậy không nhận các byte thuộc segment sau. Đây không phải packet loss ở network.

## Why the fix works
Reference `solution/Program.cs` dùng `payload.ToArray()` trước khi giải mã. Với payload telemetry nhỏ đã giới hạn kích thước, bản sao liên tục bảo toàn toàn bộ byte bất kể segmentation. Trong production cần giới hạn `payload.Length` trước khi cấp phát.

## How to verify
Sau khi tự sửa `starter/Program.cs`, chạy `./scripts/verify.ps1`. Cả single-segment, multi-segment và byte-at-a-time phải PASS. Có thể kiểm tra reference độc lập: `dotnet run --project solution/TelemetryLab.csproj -- verify`. Không thay thế kiểm thử starter bằng kiểm thử reference.

## Alternative fixes
Dùng `SequenceReader<byte>` để parse theo delimiter qua các segment và tránh allocation lớn; hoặc `foreach (var memory in payload)` với decoder duy trì trạng thái qua ranh giới byte UTF-8. Chọn theo throughput, kích thước frame và yêu cầu streaming.

## Wrong or misleading fixes
Tăng receive buffer chỉ làm lỗi ít xuất hiện, không bảo đảm sequence một segment. Bỏ bản ghi không parse được làm mất dữ liệu im lặng. Chỉ kiểm tra fixture một segment sẽ che regression. Gộp mọi payload không giới hạn vào mảng lớn có nguy cơ memory amplification.

## Production implications
Ghi `payload.Length`, số segment, số record và correlation ID; không log nội dung nhạy cảm. Đặt giới hạn kích thước và xử lý malformed frame rõ ràng.

## Trade-offs
`ToArray` đơn giản, dễ audit nhưng cấp phát và copy O(n). `SequenceReader` ít copy hơn nhưng parser phức tạp, cần test delimiter cắt giữa segment, UTF-8 nhiều byte và frame chưa đủ.

## What a Senior engineer should notice
Transport được phép thay đổi segmentation mà không đổi nội dung logic. Contract phải dựa vào byte sequence đầy đủ, không dựa vào layout bộ nhớ cụ thể. Test property này độc lập với timing và network.
