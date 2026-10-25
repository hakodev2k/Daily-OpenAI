# Before — starter gốc
- `./reproduce.ps1` exit 0 khi xác nhận lỗi có thật.
- Base: `https://partner.example.test/carrier/labels/v2/`.
- POST quan sát: `/labels/submit` → HTTP 404.
- GET quan sát: `/carrier/labels/v2/labels/status/SHIP-42` → HTTP 200.
- `./verify.ps1` thất bại (exit nonzero). Đây là trạng thái dự kiến trước khi sửa.
