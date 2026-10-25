# Trước khi sửa
- SKU hợp lệ như PART2026 trả true; PART-2026 trả false.
- SKU gồm 512 ký tự A rồi dấu ! gây RegexMatchTimeoutException trên starter gốc (budget 100 ms); reproduce.ps1 exit 0 nếu bắt được exception.
- --verify báo ít nhất một case FAIL vì timeout, không chỉ vì dữ liệu sai.
- Không đòi hỏi một mốc thời gian chạy chính xác trên mọi CPU; nếu môi trường có tối ưu regex khác biệt, ghi lại runtime/SDK và đầu ra, không tự nhận đã reproduce.
