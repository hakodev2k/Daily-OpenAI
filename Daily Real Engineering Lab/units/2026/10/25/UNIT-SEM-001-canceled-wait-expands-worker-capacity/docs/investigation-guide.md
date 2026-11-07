# Investigation guide (không mở solution trước)
1. Vẽ timeline A/B/C, đánh dấu thời điểm B yêu cầu hủy và thời điểm C được gọi.
2. Đọc code `starter/WorkGate.cs`; xác định nơi capacity được kiểm soát và nơi callback render chạy.
3. Xem `checks/Program.cs` để hiểu vì sao phép thử không phụ thuộc delay tùy ý.
4. Đặt breakpoint ở các nhánh thành công, cancellation và exception. Ghi thứ tự thay đổi trạng thái.
5. Hypothesis mẫu cần kiểm tra: callback có chạy trước khi được phép? token B có bị bỏ qua? cleanup có tương ứng với tài nguyên đã được cấp? C có vào chỉ vì A hoàn tất sớm?
6. Thử thay đổi thứ tự sự kiện hoặc cho renderer ném exception; so sánh kết quả trước/sau.
7. Chỉ kết luận sau khi test chứng minh invariant cả trên success, cancellation và fault paths.
