# Incident evidence — synthetic

- Firmware v1: `{deviceId,recordedAt,temperatureC}` (number, °C).
- Firmware v2: `{deviceId,recordedAt,celsius,schemaVersion:2}` (number, °C), cùng topic.
- Consumer A v1 ở 60% site: chỉ nhận `temperatureC`; thiếu trường thì ghi debug rồi ACK, không DLQ.
- Consumer B v2 ở 40% site: nhận `celsius`, fallback `temperatureC`.
- Alert nếu event có nhiệt độ > 6°C; SLO phát cảnh báo trong 60 giây.
- 10:01:30 BK-17 phát 8.4°C sau upgrade; không thấy alert trên consumer A.
- 10:02:10 ingress ACK; broker CPU 22%, p95 publish 18 ms; chưa có số liệu consumer end-to-end.
- 10:05 replay event v2 tới A vẫn không thấy alert.
- 90 ngày retention, 21 ngày firmware coexistence. Dữ liệu là giả lập, không chứa bệnh nhân.

Hãy phân biệt delivery, parsing, contract, threshold, timestamp; đừng suy diễn SLO từ sáu fixture.
