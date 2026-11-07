# Evidence mô phỏng (không phải kết quả chạy thực)

| Tín hiệu | Request boundary | Scanner task | Quan sát |
|---|---|---|---|
| Caller ngắt kết nối | cancellation callback đã chạy | pending | scan không giải phóng slot |
| Host shutdown | shutdown token đã canceled | pending | worker chờ tới khi scanner tự hoàn tất |
| Không hủy | active | hoàn tất | receipt hợp lệ |

Log mô phỏng:
```text
03:01:11.104 INFO request-aborted claim=claim-1042
03:01:11.106 INFO request-scope-exited claim=claim-1042
03:01:11.406 WARN scanner-inflight claim=claim-1042 state=pending
03:02:09.001 INFO partner-health status=healthy
```

Các giả thuyết có thể cạnh tranh: fake scanner bỏ qua token; callback hủy không đến service; thao tác trả về nhưng vòng đời quản lý tài nguyên khác vòng đời task. Dùng probe để phân biệt. Không coi số timestamp là benchmark hiệu năng.
