# Synthetic incident timeline
- 08:15: API trả HTTP 200, xuất 8 đăng ký.
- 08:17: Người vận hành báo spreadsheet cảnh báo một số cell.
- 08:20: CSV parser báo 8/8 dòng hợp lệ.
- 08:24: Tệp chứa một field có dấu phẩy, dấu ngoặc kép và newline vẫn được parse chính xác.
- 08:30: Nhóm điều tra yêu cầu cell-level risk evidence, không chạy công thức.
- Nhiễu: API latency 45 ms và HTTP 5xx = 0; hai metric này không chứng minh an toàn của export.
