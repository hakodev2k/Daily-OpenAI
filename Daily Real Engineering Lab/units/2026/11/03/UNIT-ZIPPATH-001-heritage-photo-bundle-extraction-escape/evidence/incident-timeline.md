# Incident timeline (synthetic)

| Time | Event | Observation |
|---|---|---|
| 00:42:10 | H-482 upload | 2 entries, HTTP 202 |
| 00:42:11 | Extraction worker | Batch marked processed |
| 00:42:12 | File monitor | Unexpected `escaped.txt` adjacent to `extract/` |
| 00:43:01 | Disk monitor | 71% free (noise, not a proven cause) |
| 00:45:18 | Isolated local retry | Unexpected file appears again |

Đây là dữ liệu mô phỏng, không phải log production. Dùng local harness để xác nhận đường dẫn và bác bỏ giả thuyết.
