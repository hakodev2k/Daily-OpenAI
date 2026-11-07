# Trước khi sửa

`reproduce.ps1` phải trả exit code 0 vì đã **phát hiện lỗi baseline**. `pendingCorrect=True`, `noEarlySend=True`, nhưng bốn bản ghi giao nhận đều mang `R-204/ranger-dung` thay vì bốn cặp độc lập. `verify.ps1` trên starter chưa sửa phải trả lỗi.

Nếu không reproduce được: chạy `setup.ps1`, kiểm tra SDK net8.0 và xác nhận không chỉnh sửa `starter/Baseline/BaselineScheduler.cs` hoặc `starter/Program.cs`.
