# Hint 02 — Ranh giới concurrency

Một phép đọc/ghi thread-safe riêng lẻ chưa đồng nghĩa toàn bộ thao tác “kiểm tra rồi lấy dữ liệu” là một transaction. Hãy vẽ timeline của hai request cùng key, đồng thời kiểm tra hai key khác nhau có cần chờ nhau không.
