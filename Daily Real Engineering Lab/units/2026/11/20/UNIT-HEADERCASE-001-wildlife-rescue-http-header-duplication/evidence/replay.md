# Evidence giả lập
| Case | Raw values | Starter observation | Contract expected |
|---|---|---|---|
| single | [north] | ROUTE:north | ROUTE:north |
| same-duplicate | [north,north] | ROUTE:north | REJECT |
| conflict | [north,south] | ROUTE:north | REJECT |
| missing | [] | REJECT | REJECT |
| empty | [""] | REJECT | REJECT |

Replay sử dụng `StringValues` trực tiếp, không mô phỏng reverse proxy thực.
