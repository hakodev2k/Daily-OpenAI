# After — learner-edited starter

- `./verify.ps1` exit 0.
- Output: `Open: WW-101,WW-103` và `Closed: WW-102`.
- Không có exception khi hai nhánh truy vấn được cho chồng lấn.
- Dữ liệu và trạng thái giữ nguyên; không có ticket thiếu, trùng hoặc phân loại sai.
- `./reproduce.ps1` sau khi sửa sẽ exit khác 0 vì symptom cũ không còn — đó là kỳ vọng; dùng `verify.ps1` làm acceptance gate.
