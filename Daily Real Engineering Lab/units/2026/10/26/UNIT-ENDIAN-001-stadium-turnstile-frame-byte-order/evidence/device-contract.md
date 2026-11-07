# Evidence — vendor integration contract

Mỗi bản tin dài chính xác 8 octet:

| Offset | Length | Field |
|---|---:|---|
| 0 | 1 | Magic `A5` |
| 1 | 1 | Version `01` |
| 2–3 | 2 | Unsigned 16-bit GateId, **network byte order (big-endian)** |
| 4–5 | 2 | Unsigned 16-bit EntryCount, **network byte order (big-endian)** |
| 6–7 | 2 | Reserved `0000` |

EntryCount là số lượt offline trong một kỳ, không phải tổng lượt cả ngày.

| Sample | Raw hex | Vendor GateId | Vendor EntryCount |
|---|---|---:|---:|
| A | `A5 01 00 2A 03 E8 00 00` | 42 | 1000 |
| B | `A5 01 00 07 01 04 00 00` | 7 | 260 |
| C | `A5 01 00 01 FF FF 00 00` | 1 | 65535 |

Gateway báo A thành 10752/59395 và B thành 1792/1025. Thiết bị khẳng định không mất byte và header hợp lệ. Hãy thử bác bỏ các hypothesis về truncation, version, offset và representation.
