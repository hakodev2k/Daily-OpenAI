# Sau sửa
- `./verify.ps1` exit 0, báo `formula-like parsed text cells=0`.
- Sáu DisplayName thuộc nhóm nguy cơ được export theo text policy; dữ liệu gốc không đổi.
- Normal names, comma, quote, newline và numeric Amount vẫn round-trip.
- `./reproduce.ps1` có thể exit khác 0 vì baseline đã được khắc phục.
- Cần UAT với spreadsheet clients thực tế; CSV không áp đặt cell type liên-client.
