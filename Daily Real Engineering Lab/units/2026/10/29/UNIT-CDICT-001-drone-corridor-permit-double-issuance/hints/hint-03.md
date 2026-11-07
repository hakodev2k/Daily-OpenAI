# Hint 3 — Trì hoãn side effect
Thử nghĩ đến một value wrapper mà việc tạo wrapper không gọi external authority; chỉ value thực sự được lưu mới thực hiện công việc, và nhiều thread đọc cùng value chỉ khởi tạo một lần. Xem `Lazy<T>` và `LazyThreadSafetyMode.ExecutionAndPublication`.
