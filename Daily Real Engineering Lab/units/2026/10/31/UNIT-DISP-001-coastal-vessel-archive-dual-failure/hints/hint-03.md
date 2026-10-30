# Hint 03
Thử tách hai pha thành các `try/catch` độc lập, lưu `ExceptionDispatchInfo` cho từng lỗi, luôn thực hiện cleanup đúng một lần, rồi quyết định chính sách: 0 lỗi, 1 lỗi, hoặc cả 2 lỗi. Không dùng `throw ex` để phát lại lỗi đơn.
