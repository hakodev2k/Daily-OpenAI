# Reference Solution — SPOILER

Chỉ đọc sau khi reproduce và tự đề xuất cách sửa.

## 1. Symptoms
Cổng đối tác từ chối file .gz dù một số decoder vẫn đọc được nội dung. Trường hợp rỗng cũng có thể thất bại.

## 2. Evidence
GZIP có header, dữ liệu có thể được giải mã, nhưng 8 byte trailer không đúng hợp đồng: CRC32 và ISIZE. Harness tính trailer độc lập từ payload đầu vào; không dùng source solution làm oracle.

## 3. Root cause
Trong starter, `using var compressor` chỉ gọi `Dispose()` khi rời scope. Biểu thức `return output.ToArray()` được đánh giá **trước** khi các `using var` được dispose. Vì vậy byte[] snapshot được lấy khi `GZipStream` chưa kết thúc và chưa ghi đầy đủ trailer GZIP. `Flush()` chỉ đẩy dữ liệu đang đệm; nó không hoàn tất stream và không tương đương `Dispose()`.

## 4. Why the fix works
Tách scope của compressor bằng `using (...)` rồi chỉ gọi `output.ToArray()` **sau khi** compressor đã dispose. `leaveOpen: true` cho phép đọc buffer sau khi compressor hoàn tất. Mã tham khảo: [ArchiveExporter.cs](ArchiveExporter.cs).

## 5. How to verify
Thay nội dung `starter/ArchiveExporter.cs` bằng implementation của reference rồi chạy `pwsh -File ./verify.ps1` từ unit root. Harness compile trực tiếp file trong starter, không compile reference. Cả payload rỗng và không rỗng phải có header/trailer/roundtrip hợp lệ. Sau sửa, reproduce mode phải không còn tìm thấy lỗi.

## 6. Alternative fixes
- Đóng compressor rõ ràng trước khi snapshot; dùng `Dispose()` có chủ đích nếu ownership được ghi rõ, nhưng scoped `using` dễ review hơn.
- Với batch lớn, stream thẳng vào file tạm hoặc object staging; đóng compressor, xác thực artifact, rồi atomic rename/publish. Cần thiết kế cleanup khi exception và chính sách idempotency.

## 7. Wrong or misleading fixes
- Gọi `Flush()` nhiều lần: không bảo đảm trailer được ghi.
- Bỏ kiểm tra footer vì giải nén thành công: có thể chấp nhận artifact không đạt định dạng.
- Thêm retry upload: không chữa dữ liệu đã hỏng trước khi gửi.
- Gọi `output.ToArray()` sau khi return bằng callback/background task: không thay đổi byte[] đã chụp.

## 8. Production implications
Tách trạng thái `writing`, `finalized`, `validated`, `published`. Không publish object khi compressor còn mở. Ghi kích thước, checksum và trạng thái hoàn tất; nếu có lỗi giữa chừng, xóa staging artifact.

## 9. Trade-offs
MemoryStream phù hợp bài nhỏ nhưng copy buffer và dùng RAM theo kích thước artifact. Streaming giảm memory nhưng cần chiến lược temp object, commit marker và retry an toàn. Không nên thay đổi định dạng chỉ để né vòng đời stream.

## 10. Senior engineer should notice
`Dispose` là một phần của giao thức tạo artifact, không chỉ cleanup. Kiểm thử phải xác nhận **format-level integrity**, không chỉ business payload roundtrip. Thứ tự finalize → validate → publish là invariant ở ranh giới hệ thống.
