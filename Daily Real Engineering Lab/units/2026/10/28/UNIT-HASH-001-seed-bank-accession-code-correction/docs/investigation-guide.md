# Investigation guide
1. Chạy `./reproduce.ps1` và lưu 4 dòng chứng cứ.
2. So sánh các code path `CorrectCode`, `ContainsCode`, `Register`, `RemoveCode`.
3. Tự kiểm tra một object trước và sau correction: dữ liệu hiển thị có giống kết quả lookup không?
4. Đặt giả thuyết về hợp đồng collection và chứng minh bằng code hoặc debugger.
5. Chỉ sửa `starter/AccessionRegistry.cs`, giữ nguyên public API.
6. Chạy `./verify.ps1`, rồi xem reference solution.
