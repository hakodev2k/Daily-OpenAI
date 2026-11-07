# Trước khi sửa

- `./reproduce.ps1` exit 0 khi health trả 204 có `X-Trace-Id`, live trả 200 SSE đúng payload nhưng thiếu header.
- `./verify.ps1` exit khác 0, hiển thị `FAIL`.
- `./run.ps1` vẫn có thể exit 0 vì API trả đúng status và body; đây chưa phải bằng chứng trace contract đạt.
- Nếu chưa tái hiện, chạy lại setup và kiểm tra mã nguồn starter còn nguyên.
