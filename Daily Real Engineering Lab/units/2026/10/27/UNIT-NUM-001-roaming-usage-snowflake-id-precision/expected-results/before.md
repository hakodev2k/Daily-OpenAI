# Before — Starter
`./reproduce.ps1` phải in ít nhất một `CHECK_FAILED:` và xác nhận **EXPECTED FAILURE**; `ordinary-ids-and-duplicate` vẫn có thể `CHECK_OK`. Fixture chính mong đợi ID `9007199254740992` và `9007199254740993` cùng tổng 70 units nhưng starter chỉ nhận một event và tính thiếu. Nếu lệnh không build được, đó là lỗi môi trường, **không phải** reproduce hợp lệ.
