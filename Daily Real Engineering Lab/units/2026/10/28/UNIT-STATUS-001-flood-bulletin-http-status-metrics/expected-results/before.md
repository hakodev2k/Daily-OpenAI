# Before

- `./reproduce.ps1` exits 0 and prints four client/metric pairs.
- Client statuses: 200, 429, 503, 404.
- Metrics: 200, 200, 200, 200.
- `Response contract: PASS`; `Metric matches client: FAIL`.
- `./verify.ps1` exits nonzero; this is the intended initial regression failure.
