# Incident evidence — dữ liệu tổng hợp cho lab

Triệu chứng quan sát từ cùng một process sau tối ưu tài nguyên:

| Thứ tự | Requested tenant | Expected open code | Observed open code | HTTP-equivalent outcome |
|---|---|---|---|---|
| 1 | north | N-101 | N-101 | 200 |
| 2 | south | S-202 | N-101 | 200 |
| 3 | new-tenant | (empty) | N-101 | 200 |
| 4 | north | N-101 | N-101 | 200 |

- SQLite seed gồm N-101 (north, open), N-102 (north, closed), S-202 (south, open).
- Không có database exception; số bản ghi seed không đổi.
- CPU, DB query duration và số request retry không có dấu hiệu tăng bất thường (noise, không chứng minh tính đúng).
- Dữ liệu trên là evidence giả lập để định hướng điều tra, không phải log production thực tế.
- Để có bằng chứng runtime, chạy harness và ghi lại output thực tế. Thử đổi thứ tự các case sau khi lưu baseline.
