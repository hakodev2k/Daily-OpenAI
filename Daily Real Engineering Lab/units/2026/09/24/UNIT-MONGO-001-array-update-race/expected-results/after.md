# After
- Final state luôn chứa `owner`, `alice`, `bob` sau concurrent operations.
- Repeating cùng membership intent không tạo duplicate.
- Fix không phụ thuộc vào timing/delay cụ thể và boundary concurrency nằm ở storage mutation thay vì process-wide application lock.