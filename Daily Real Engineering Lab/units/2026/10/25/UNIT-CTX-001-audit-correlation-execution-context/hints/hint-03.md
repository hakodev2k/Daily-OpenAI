# Hint 03 — khái niệm sát lời giải

`ThreadPool.UnsafeQueueUserWorkItem` không tự flow `ExecutionContext` của caller. `AsyncLocal<T>` nằm trong ngữ cảnh đó. Xác định correlation ID tại boundary dispatch và truyền rõ ràng cùng job; tránh dựa vào ambient state trong callback.
