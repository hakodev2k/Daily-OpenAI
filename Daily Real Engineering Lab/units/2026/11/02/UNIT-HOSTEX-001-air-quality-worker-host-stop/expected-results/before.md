# Expected Results — Starter

Sau `./reproduce.ps1`, chương trình starter xuất các tín hiệu:

```text
hostStoppedEarly=True
attempts=AQ-101,AQ-102
published=AQ-101
failed=
LAB_RESULT=EXPECTED_FAILURE
```

Generic Host có thể log lỗi `BackgroundService` và việc dừng host. Chương trình starter trả exit code 3; `reproduce.ps1` chuyển điều đó thành exit code 0 **chỉ để xác nhận reproduce thành công**. Không dùng thời gian tuyệt đối làm tiêu chí pass.
