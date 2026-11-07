# Bằng chứng từ môi trường thử nghiệm

Nhóm support trích xuất các request độc lập, cùng phiên bản API. Dưới đây là dữ liệu **mô phỏng**, không phải log production.

| Request | Accept-Language | Locale API trả về | Ghi nhận của QA |
|---|---|---|---|
| R-201 | `en;q=0.2, vi;q=0.9` | en | Ứng dụng đặt tiếng Việt ở ưu tiên cao |
| R-202 | `vi;q=0, en;q=1` | vi | Client đánh dấu tiếng Việt không được chấp nhận |
| R-203 | `fr-CA, en;q=0.8` | en | Kho caption có tiếng Pháp |
| R-204 | `de, fr;q=0.7` | en | Kho không có tiếng Đức nhưng có tiếng Pháp |
| R-205 | (không gửi header) | en | Hành vi mặc định vẫn bình thường |

Tín hiệu nhiễu: CPU 22–27%, HTTP 5xx = 0, độ trễ trung vị 8 ms. Chưa có bằng chứng về lỗi cache hay mất file caption.

Hãy tách lỗi *chọn locale* khỏi lỗi *tải nội dung*. Mỗi giả thuyết cần có phép thử phản chứng.
