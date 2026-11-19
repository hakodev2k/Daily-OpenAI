# Investigation worksheet

1. Chạy reproduce và ghi thời gian, exit state, hai dòng BEGIN.
2. Hypothesis A: CPU-bound loop? Hypothesis B: wait-for-resource? Hypothesis C: I/O blocked? Nêu bằng chứng phân biệt.
3. Vẽ wait-for graph: worker, lock đã giữ, lock đang đợi.
4. Đề xuất invariant: tổng vé 200, số dư không âm, chuyển 10/20 phải áp dụng đúng một lần.
5. Thay đổi starter, chạy verify. Thử 20 lần và ghi nhận tính ổn định (không thay thế stress test).
6. So sánh với solution, giải thích giới hạn khi chuyển sang database đa tiến trình.

Lưu ý: kết quả gốc có tính xác định do barrier; bản sửa phải loại bỏ cơ chế tái hiện gây giữ khóa vô hạn.

