# QA evidence (synthetic fixture, chưa phải log chạy thực tế)

Bối cảnh: vé mở khóa xe đạp có hiệu lực từ T-5 phút đến T+15 phút. Harness điều khiển đồng hồ mô phỏng tại T, T+14:59, T+15:00, T-5:00 trừ một tick, rồi quay về T.

| Case | Contract | Quan sát starter dự kiến |
|---|---|---|
| T | Allowed | NotYetValid |
| T+14:59 | Allowed | NotYetValid |
| T+15:00 | Expired | NotYetValid |
| trước T-5:00 | NotYetValid | NotYetValid |
| start == end | InvalidWindow | InvalidWindow |
| quay về T | Allowed | NotYetValid |

Tất cả mốc dùng `DateTimeOffset` UTC. Đây là fixture kỳ vọng qua static review; không tuyên bố đã chạy chương trình. Dùng `run.ps1` để thu log thực tế.
