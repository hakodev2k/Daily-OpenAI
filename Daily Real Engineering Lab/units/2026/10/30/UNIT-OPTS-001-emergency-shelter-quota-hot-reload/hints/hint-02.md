# Hint 2 — Kiểm tra lifecycle
Một singleton service tồn tại qua nhiều lần `configuration.Reload()`. Hãy xác định field nào được gán khi constructor chạy và liệu các field đó có thể thay đổi sau reload không. `IOptionsMonitor<T>` và giá trị mà consumer giữ có phải cùng một thứ?
