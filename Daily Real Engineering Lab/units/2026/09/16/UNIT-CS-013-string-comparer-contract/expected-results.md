# Expected Results

## Before
- `SKU-ALPHA` resolve `Product-101`.
- `sku-alpha` và `Sku-Beta` không resolve dù upstream xem chúng tương đương với key đã đăng ký.
- `SKU-GAMMA` không resolve.

## After
- Ba identifier hợp lệ resolve đúng mapping theo business contract.
- `SKU-GAMMA` vẫn không resolve.

Nếu không reproduce được, kiểm tra đang chạy project trong `starter/` và chưa sửa starter trước khi chạy `reproduce.ps1`.