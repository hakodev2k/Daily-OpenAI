# Acceptance matrix

| Case | Situation | Expected |
|---|---|---|
| A | LOT-A17 projection r11; Registry r12 RECALLED | HOLD |
| B | Duplicate recall r12 | HOLD; idempotent |
| C | APPROVED r11 arrives after RECALLED r12 | HOLD; never regress revision |
| D | Registry timeout, no fresh proof | HOLD; audit |
| E | LOT-B08 r7 APPROVED, authoritative check succeeds | ALLOW |
| F | Retry same dispatchId | One dispatch only |
| G | 400 recalls, projection lag 10 min | All recalled lots HOLD |

Giải thích bằng chứng nào đủ fresh, xử lý race giữa authoritative check và physical release, và hệ quả availability của fail-closed. Token approval cấp trước recall không mặc nhiên an toàn.
