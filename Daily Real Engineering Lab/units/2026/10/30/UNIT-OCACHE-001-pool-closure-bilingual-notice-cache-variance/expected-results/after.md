# Expected Results — After

`verify.ps1` phải exit 0 sau khi sửa `starter/Program.cs`. Các request `vi-VN` có `language=vi`, `en-US` có `language=en`; mỗi biến thể lặp lại giữ nguyên `renderCount` của chính nó; hai biến thể có số render khác nhau. Endpoint `/health` vẫn trả healthy. `reproduce.ps1` lúc này phải exit 1 vì symptom ban đầu không còn.
