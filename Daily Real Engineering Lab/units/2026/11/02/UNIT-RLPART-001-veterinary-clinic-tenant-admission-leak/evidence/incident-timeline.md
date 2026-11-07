# Incident evidence — mô phỏng

Hợp đồng: 2 yêu cầu / 10 phút **cho mỗi phòng khám**, gộp hai endpoint. Clinic claim do authentication fixture thiết lập. Header `X-Clinic-ID` là input không đáng tin.

| ID | Clinic claim | X-Clinic-ID | Path | Quan sát | Kỳ vọng |
|---|---|---|---|---|---|
| A1 | Alpha | Alpha | /api/vaccinations | 200 | 200 |
| A2 | Alpha | Alpha | /api/vaccinations | 200 | 200 |
| B1 | Beta | Beta | /api/vaccinations | 429 | 200 |
| A3 | Alpha | Beta | /api/certificates | 200 | 429 |
| B2 | Beta | Beta | /api/certificates | 200 | 200 |
| B3 | Beta | Beta | /api/vaccinations | 429 | 429 |

Noise: CPU, latency downstream và DNS bình thường; không có retry. Cả sáu quyết định trong một process, một cửa sổ, theo thứ tự bảng. Phân tích A2/B1, A2/A3 và A3/B2.
