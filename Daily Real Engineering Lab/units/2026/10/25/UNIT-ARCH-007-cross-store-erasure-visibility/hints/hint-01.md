# Hint 01 — Xác định lời hứa với người dùng

Không bắt đầu bằng chọn queue hoặc database. Viết một câu bất biến: sau khi người dùng nhận trạng thái COMPLETED, các đường public read nào bắt buộc không trả dữ liệu? Kiểm tra từng đường bằng timeline và data-flow inventory. Tách visibility khỏi physical retention.
