# After — learner sửa
- ID hợp lệ, kể cả chuỗi dài, được chấp nhận.
- ID có dấu gạch ngang, ID rỗng, và ID rất dài với ký tự cuối không hợp lệ đều bị từ chối.
- Không có `RegexMatchTimeoutException` trong tập regression.
- `./verify.ps1` ghi `VERIFICATION_PASSED` và trả exit code 0 trên **starter đã sửa**.
- Giữ bounded execution policy; nếu mở rộng grammar, thêm test cho các chuỗi gần khớp và đầu vào đối nghịch.
