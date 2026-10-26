# After — learner workspace

- Cả ba fixture (10, 12, 1 byte) có `downloadedBytes=sourceBytes` và `sameBytes=True`.
- Các request không chồng lấn và bao phủ toàn bộ byte offsets đúng một lần.
- `verify.ps1` exit 0 khi sửa đúng trong `starter/RangeLab/ChunkDownloader.cs`.
- Regression quan trọng: trường hợp tổng chiều dài chia hết cho chunk size, không chia hết và chỉ có một chunk.
- Production: kiểm tra `Content-Range`, tổng length, short-read và checksum nếu tải qua mạng thật.
