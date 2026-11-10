# Verification matrix

| Case | Fault injection | Required invariant |
|---|---|---|
| V1 | Upload ảnh ba lần | Idempotent receipt |
| V2 | Hai thiết bị sửa annotation offline | Không mất revision |
| V3 | Operator sửa một byte trong blob | Export verify fail |
| V4 | Đồng hồ thiết bị lệch hai giờ | Không dùng local time làm thứ tự đáng tin duy nhất |
| V5 | Export lại sau khi sync | Export cũ vẫn verify; revision liên kết |
| V6 | Key bị thu hồi | Có incident/key-version procedure |
| V7 | Upload đứt giữa chừng | Partial không được verified |
| V8 | User chỉ có quyền redacted | Không lộ original/EXIF |

Với mỗi case, ghi PASS/FAIL, bằng chứng, rủi ro tồn dư. Thêm cost, blast radius và restore drill.
