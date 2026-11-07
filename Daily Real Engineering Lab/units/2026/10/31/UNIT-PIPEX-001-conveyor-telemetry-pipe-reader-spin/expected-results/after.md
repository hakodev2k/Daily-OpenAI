# After

- `./verify.ps1`: exit 0 và in `VERIFIED: split frame reconstructed once...`.
- Parser đợi dữ liệu mới và trả đúng `CONVEYOR|42`.
- `FrameProbe.Count == 1`; không có spin guard.
- `./reproduce.ps1` có thể exit 1 sau khi sửa vì baseline symptom đã biến mất.
