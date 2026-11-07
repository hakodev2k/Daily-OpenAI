# Expected Results — Before

Trên starter nguyên bản, `reproduce.ps1` dự kiến exit 0: request đầu tiên `vi-VN` trả `language=vi`; request `en-US` sau đó cũng trả `language=vi` và cùng `renderCount`. Hai request tiếp theo tiếp tục nhận biến thể đầu tiên. `verify.ps1` phải exit 1. Đây là kỳ vọng từ static review, chưa phải kết quả chạy thực tế.
