# Investigation guide

1. Chạy `./reproduce.ps1`, lưu output và exit code.
2. Chạy `./verify.ps1` trước khi sửa; phân loại case nào fail.
3. Đọc `FragmentedStream` trong `starter/Program.cs`, đối chiếu tổng số byte và giới hạn mỗi lần đọc.
4. Viết 2–3 hypothesis: nguồn thiếu byte, parse endian sai, hay decoder diễn giải sai hành vi I/O. Đề xuất cách bác bỏ từng giả thuyết.
5. Chỉ sửa `starter/FrameDecoder.cs`, không sửa test double hoặc baseline.
6. Kiểm tra EOF thật, payload zero, length invalid và cancellation; ghi trade-off của phương án chọn.

Không dùng dữ liệu production hoặc thiết bị đường sắt thật; toàn bộ là fixture cục bộ.
