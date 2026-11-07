# Incident timeline — synthetic, trước sửa

- 10:01 — worker tạo một outbound partner adapter dùng lại trong process.
- 10:02 — tenant Alpha authenticate; partner trả 200 và cookie session giả lập.
- 10:03 — Alpha quote trả 200.
- 10:04 — Beta chưa authenticate nhưng quote trả 403, thay vì 401. Marker quan sát tại fake partner là `alpha`.
- 10:05 — Beta authenticate; quote của Beta trả 200.
- 10:06 — Alpha quote trả 403, marker `beta`.

## Bằng chứng và nhiễu

- Partner quote route hoạt động cho cả hai tenant khi session hợp lệ; giả thuyết URL sai không giải thích được thứ tự.
- Network trên loopback, không có 429/503 hay timeout.
- CPU và memory ổn định; không có dấu hiệu ThreadPool starvation.
- Chỉ dùng diagnostic tenant marker giả lập; tuyệt đối không log giá trị session cookie thật.

## Yêu cầu

Đề xuất 3 hypothesis; với mỗi hypothesis ghi một quan sát có thể bác bỏ. Sau đó chạy `verify.ps1` để kiểm tra cô lập hai chiều.
