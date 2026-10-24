# Incident brief — cohort roster

Báo cáo production (dữ liệu mô phỏng, không có PII):

| Request shape | Prior query in same DbContext | Expected active enrollment IDs | Observed |
|---|---|---|---|
| R1 — cohort report | None | 101, 103 | 101, 103 |
| R2 — dashboard + cohort report | Load all enrollments | 101, 103 | 101, 102, 103 |
| R3 — cohort report (new scope) | None | 101, 103 | 101, 103 |

Dữ liệu enrollment: 101 active, 102 inactive, 103 active. Không có transaction ghi giữa các request. DBA xác nhận SQL có điều kiện active trong đường report. Hãy kiểm chứng sự khác biệt giữa rowset và collection runtime.
