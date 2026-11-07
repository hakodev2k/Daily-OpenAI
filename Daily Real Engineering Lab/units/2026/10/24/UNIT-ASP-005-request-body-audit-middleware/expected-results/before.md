# Before
Trên starter gốc, reproduce trả exit code 0. Hai shipment hợp lệ trả 400 / invalid-json mặc dù auditBytes đúng độ dài UTF-8. Malformed JSON và body rỗng trả 400. Verify trên starter gốc trả exit code 1. Nếu không reproduce được, kiểm tra .NET SDK và bảo đảm middleware chưa được sửa.
