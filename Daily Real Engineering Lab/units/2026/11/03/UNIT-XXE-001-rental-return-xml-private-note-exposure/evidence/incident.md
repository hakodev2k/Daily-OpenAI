# Incident evidence (mô phỏng, chưa chạy tại đây)
- 09:14 — Nhận `ReturnReceipt` từ đối tác thuê máy xúc; `ContractId=RNT-042`.
- 09:15 — Mẫu XML của đối tác chứa một phần khai báo trước root và `Note` không chứa văn bản nhạy cảm dưới dạng literal.
- 09:15 — Sau import, trường `Note` chứa `LAB-CANARY-NOT-A-REAL-SECRET`.
- 09:16 — Một phiếu XML đơn giản vẫn import thành công; lỗi không xảy ra cho mọi phiếu.
- 09:17 — Metrics CPU thấp, không có retry từ HTTP gateway (noise; không giải thích nguồn nội dung).
- Output **dự kiến** trước sửa: `VALID_RECEIPT=PASS` và `EXTERNAL_ENTITY=EXPANDED`.

Mọi giá trị chỉ là fixture mô phỏng; không có log production hoặc secret thực.
