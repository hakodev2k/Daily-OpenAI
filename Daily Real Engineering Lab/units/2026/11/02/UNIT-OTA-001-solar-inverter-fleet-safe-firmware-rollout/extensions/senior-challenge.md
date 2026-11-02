# Senior challenge — requirement changes

1. North offline 8 giờ; Central/South vẫn hoạt động. Chứng minh regional isolation, no raw-log cross-region và resume không phát stale commands.
2. Firmware signing key bị nghi lộ khi 2.000 devices offline. Thiết kế revocation, trust-anchor rotation và recovery single-bank mà không brick fleet.
3. Egress giảm từ 320 xuống 160 GiB/day. Tính lower bound mới; chọn delta-only, edge caching hoặc thay đổi SLA và ghi ADR.

Nêu điều kiện khiến bạn chuyển từ Option B sang managed OTA, cùng số liệu cần đo.
