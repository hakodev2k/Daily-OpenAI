# Prerequisite (không spoil)

- Biết `Task<T>`, `await`, `ConcurrentDictionary` và `DateTimeOffset`.
- Hiểu cache hit/miss và TTL ở mức khái niệm.
- Phân biệt correctness của response với lượng tài nguyên tiêu thụ.
- Đọc `ScenarioResult` trong contract để hiểu cách đo, không chỉnh contract.

Để thay đổi logic, chỉ chỉnh `starter/TariffCache.cs`. Các file khác tạo ra môi trường đo ổn định.
