# Before
`CALLS 2`, `CACHE 1`, `SAME True`, `FAILURE_RETRY True`. Hai allocation đã được ghi nhận dù cache chỉ có một winner. Nếu timeout 10 giây, kiểm tra máy có đủ ThreadPool workers và không chạy đồng thời nhiều harness.
