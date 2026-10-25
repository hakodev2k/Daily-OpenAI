# After — learner-edited starter

- `verify.ps1` exit **0** chỉ khi cả sáu UTC instants đúng, đúng thứ tự và đúng ngày.
- `reproduce.ps1` có thể exit **1** sau khi sửa; đó là bình thường vì không còn symptom gốc.
- Không thay đổi ngày đầu vốn đúng; không sửa hard-coded expected values trong `Program.cs`.
- Giải thích chính sách xử lý giờ local bị thiếu hoặc trùng trong các ca mở rộng.
