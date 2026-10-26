# After

`./verify.ps1` kết thúc exit 0 và in `VERIFY PASS`: sau lần import đầu có 1 active `branch-a/CARD-204`, 1 tombstone `branch-a`, 1 active `branch-b`; lần import tiếp theo cập nhật đúng active row `branch-a` thành `Mai` mà không tăng row count. Không sửa harness.
