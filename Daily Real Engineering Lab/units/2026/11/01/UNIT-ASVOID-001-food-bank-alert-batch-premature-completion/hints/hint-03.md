# Hint 3 — Khái niệm

`List<T>.ForEach` nhận `Action<T>`, không nhận `Func<T, Task>`. Một `async` lambda bị chuyển thành `async void` không có `Task` để caller await. Hãy xây dựng đường đi mà mọi `Task` gửi được chờ và lỗi được truyền ra.
