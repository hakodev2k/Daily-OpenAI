# Before — starter

Chạy `./reproduce.ps1` trên starter chưa chỉnh sửa:

- Seller `ART-17` có ba sales, mỗi sales USD 0.05, feeRate 10%.
- Statement quan sát: gross USD 0.15, fee USD 0.02, net USD 0.13.
- Theo transaction ledger contract: gross USD 0.15, fee USD 0.03, net USD 0.12.
- Script in `REPRODUCED` và trả exit code 0. `./verify.ps1` phải thất bại với ít nhất một invariant.

Nếu kết quả khác, xác nhận chưa sửa starter, đang dùng SDK hỗ trợ net8.0 và chạy từ đúng unit.
