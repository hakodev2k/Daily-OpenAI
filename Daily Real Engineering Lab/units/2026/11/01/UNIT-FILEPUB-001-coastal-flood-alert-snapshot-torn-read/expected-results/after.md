# After — learner fix

- verify.ps1 thoát 0.
- DURING là **toàn bộ** revision 17 hoặc 18, không rỗng/dở dang.
- AFTER revision18=True.
- INTERRUPTED oldRevisionPreserved=True; injectedException=True.
- TEMP clean=True.
- reproduce.ps1 sau sửa có thể thoát 1 vì lỗi cũ đã biến mất.
- Nếu không tái hiện: kiểm tra quyền ghi thư mục tạm, SDK và filesystem semantics.
