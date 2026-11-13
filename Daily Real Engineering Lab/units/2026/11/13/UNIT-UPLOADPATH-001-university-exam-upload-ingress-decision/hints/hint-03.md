# Hint 03
Một upload token ngắn hạn chỉ nên có quyền ghi vào một object/key staging gắn với attempt. Finalize cần server-side kiểm tra ownership, size/hash, thời điểm và trạng thái scan; dùng conditional write hoặc unique constraint để chống finalize lặp.
