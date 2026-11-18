# Evidence pack

- Clock cố định cho replay: `2027-01-15T08:00:00Z` tương ứng Unix seconds 1800000000.
- A-17: raw 1800000000000; expected current reading.
- B-22: raw 1800000000; expected current reading.
- A-18: raw 1800000060000; expected future reading.

**Giả thuyết cần kiểm tra:** clock drift, latency của gateway, unit mismatch, event duplication. Không kết luận từ một log duy nhất.

Đầu ra mong đợi: bảng raw value → protocol → normalized UTC → age → classification, và ít nhất hai regression test mới.
