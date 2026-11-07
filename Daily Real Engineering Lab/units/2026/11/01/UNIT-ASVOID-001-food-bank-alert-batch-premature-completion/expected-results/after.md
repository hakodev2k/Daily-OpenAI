# Sau khi sửa

`./scripts/verify.ps1` phải trả exit code 0:

```text
dispatchTaskCompleted=False
sentBeforeRelease=0
sentAfterRelease=3
PASS: completion, send count, and failure propagation
```

Không dùng thời gian sleep làm tiêu chí. Harness chủ động giữ gateway chưa hoàn tất, sau đó mở gate. Test kiểm tra trạng thái trước gate, đủ ba send sau gate và exception từ gateway được caller quan sát. `reproduce.ps1` có thể trả exit code 2 sau khi đã sửa; đó là hành vi đúng vì triệu chứng cũ không còn.
