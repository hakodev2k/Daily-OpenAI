# Investigation guide

## Trình tự
1. Chạy `reproduce.ps1`, lưu stdout và exit code.
2. Chạy `verify.ps1`, ghi rõ trường hợp nào không đạt.
3. Đọc `starter/ArchiveExporter.cs` và vẽ timeline từ lúc tạo buffer đến lúc trả byte[].
4. Đối chiếu header, trailer và roundtrip; lưu ý một decoder có thể không xác nhận trailer bị thiếu.
5. Đưa ra tối thiểu hai giả thuyết có thể kiểm chứng bằng byte-level evidence.
6. Sửa code trong `starter/`, chạy lại verify và mô tả tác động với streaming, memory, upload và lỗi giữa chừng.

## Hypothesis matrix
| Giả thuyết | Bằng chứng ủng hộ | Phép thử bác bỏ |
|---|---|---|
| Payload bị thay đổi trước khi nén | Hash/roundtrip không khớp | So byte sau giải nén |
| GZIP không hoàn chỉnh | Header có, trailer không hợp lệ | Kiểm tra CRC32/ISIZE |
| Lỗi ở đường truyền | Artifact trước upload khác artifact sau upload | So hash tại hai điểm |

## Câu hỏi production
- Nên publish object vào vị trí chính thức khi nào?
- Khi có exception giữa chừng, làm sao tránh xuất bản artifact chưa hoàn tất?
- Có nên giữ toàn bộ dữ liệu nén trong RAM khi batch lớn hơn nhiều?
- Tại sao `Flush` không phải bằng chứng kết thúc định dạng?
