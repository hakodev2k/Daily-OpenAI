# Reference — CSV record framing

Nguyên nhân: parser dùng `Split('\n')` để phân bản ghi và `Split(',')` để phân trường, trong khi CSV có thể chứa cả hai ký tự bên trong trường quoted. Khi nhận CRLF bên trong `Note`, parser biến một record thành hai và mất tính toàn vẹn.

Thay phương thức `CsvImporter.Parse` trong starter bằng bản tham khảo ở [CsvImporter.reference.cs](CsvImporter.reference.cs) (thay cả class, giữ record và test harness). Chạy `./verify.ps1` trên starter đã sửa.

Không sửa bằng cách bỏ dòng có số cột sai, replace newline toàn cục hoặc split dấu phẩy bằng regex đơn giản: các cách đó làm mất dữ liệu hoặc không xử lý được escaped quote.

Trade-off: state machine gọn và không phụ thuộc package nhưng cần regression tests đầy đủ. Với input lớn, cân nhắc streaming parser chuẩn RFC 4180, giới hạn record/field và validation schema tại ingestion boundary. Bộ test này không bao phủ mọi biến thể CSV; production cần kiểm soát encoding, kích thước và policy đối với malformed records.
