# Hint 3 — Hai policy khác nhau
CSV escaping chỉ bảo vệ cấu trúc field. Cần thêm text-cell policy cho đúng cột trước bước serialize, rồi vẫn escape CSV. So sánh với XLSX có cell type tường minh.
