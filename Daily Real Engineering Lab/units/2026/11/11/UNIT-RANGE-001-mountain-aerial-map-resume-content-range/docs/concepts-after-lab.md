# Concepts sau lab
HTTP Range response mô tả phạm vi byte inclusive. Với resume từ N byte, response cần bắt đầu tại N, tổng chiều dài phải phù hợp nguồn, và số byte body phải khớp phạm vi được khai báo. ETag/If-Range giúp tránh ghép hai phiên bản tài nguyên; checksum end-to-end vẫn cần cho integrity ở production. Phân biệt HTTP transport success và application-level correctness.
