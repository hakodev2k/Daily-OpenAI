# Hint 03 — Phạm vi và phiên bản

Dùng `XNamespace` tương ứng với hợp đồng `urn:energy:certificate:v2`, kiểm tra tên root theo namespace và chỉ duyệt `Meter/Reading` trực tiếp trong schema. Đừng lọc toàn bộ tài liệu chỉ bằng `Name.LocalName`; dữ liệu chẩn đoán và extension là các negative cases bắt buộc.
