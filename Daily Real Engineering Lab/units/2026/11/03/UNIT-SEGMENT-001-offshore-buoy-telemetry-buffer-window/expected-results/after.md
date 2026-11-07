# After
`./verify.ps1` exit 0 và cả 4/4 fixture PASS. Frame gồm header hai byte đúng độ dài, payload đúng thứ tự và chỉ chứa byte của message. Cửa sổ rỗng tạo `0000`. Sau khi fix, `./reproduce.ps1` exit 1 là đúng vì lỗi gốc không còn.
