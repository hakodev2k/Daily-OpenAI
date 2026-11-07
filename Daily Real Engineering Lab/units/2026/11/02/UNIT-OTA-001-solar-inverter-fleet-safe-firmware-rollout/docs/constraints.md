# Constraints

| Hạng mục | Hợp đồng |
|---|---|
| Fleet | 24.000 devices; 6.000 sites; 4/site; 8.000/region |
| Connectivity | 30% offline trong một giờ; ACK có thể delayed, duplicated, out-of-order |
| Concurrency | Tối đa 240 devices INSTALLING/VERIFYING toàn fleet; tối đa 1/site |
| Site safety | Tối thiểu 3/4 inverter/site phải hoạt động |
| Cohorts | Pilot 240 → tối đa 1.200 → tối đa 4.800 → phần còn lại, explicit gate mỗi wave |
| Failure gate | Pilot failed / (succeeded + failed) ≤ 2% sau ít nhất 100 terminal results |
| Emergency stop | Không phát lệnh **mới** sau tối đa 5 phút từ lúc xác nhận gate vi phạm |
| Firmware | Full 45 MiB; delta 12 MiB; 80% đủ điều kiện delta |
| Egress | 320 GiB/ngày toàn fleet, chưa tính retransmit/overhead |
| Hardware | 90% dual-bank; 10% single-bank không rollback tự động |
| Security | Signed manifest, hash validation, compatibility, anti-rollback, audit |
| Data residency | Raw device logs giữ trong vùng; global aggregate được phép |
| Operations | 5 operators; incremental cost ≤ 4.000 USD/tháng |
| Reporting | Offline không được coi là FAILED hoặc SUCCEEDED |

Tách hard invariants (site safety, trust, fencing) và tunable policy (wave size, threshold, timeout). Ghi mọi giả định bổ sung.
