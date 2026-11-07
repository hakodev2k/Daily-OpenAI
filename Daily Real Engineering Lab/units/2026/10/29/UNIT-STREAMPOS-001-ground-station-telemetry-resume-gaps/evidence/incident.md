# Incident evidence — simulated fixture

- Feed: 4 dòng, một event mỗi dòng, UTF-8 không BOM, kích thước nhỏ hơn 1024 byte.
- Batch limit: 1, rồi 2, rồi 2; mỗi batch tạo một instance importer mới.
- Triệu chứng mong đợi với starter: batch 1 trả GS-101; batch 2 và 3 trả rỗng.
- Không có lỗi parser hay exception; các dòng GS-102, GS-103, GS-104 vẫn còn nguyên trong file nguồn.
- Các chỉ số cần thu thập: SourceBytes, SourceLines, Cursor, AcceptedTotal, kết quả từng batch.

## Hypotheses to test
1. Nguồn bị thay đổi hoặc thiếu bản ghi.
2. Worker lưu hoặc diễn giải cursor không đúng hợp đồng.
3. Parser bỏ dòng hợp lệ.
4. Logic giới hạn batch dừng trước khi chấp nhận bản ghi.

Bằng chứng này mô tả triệu chứng; hãy tự xác định cơ chế trước khi mở reference.
