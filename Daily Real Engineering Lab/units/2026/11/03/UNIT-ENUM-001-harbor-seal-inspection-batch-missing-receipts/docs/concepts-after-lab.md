# Sau lab — mô hình tư duy

- `IEnumerable<T>` có thể là lazy, stateful hoặc destructive; việc duyệt không mặc nhiên pure hay repeatable.
- `Enumerable.Count()` có thể dùng collection count tối ưu, nhưng với sequence khác phải duyệt để đếm. Vì vậy test bằng `T[]` không đủ bảo đảm behavior của streaming adapter.
- Thiết lập invariant `Received = Stored + Rejected` cho đường chạy hoàn thành bình thường; trong production phải mô hình hóa riêng lỗi lưu trữ và partial failure.
- Kiểm thử contract bằng hai loại nguồn: replayable và one-shot, cùng input.
- Nếu cần all-or-nothing, phải thiết kế transaction/compensation riêng; chỉ đổi vòng lặp không tự tạo atomicity.
