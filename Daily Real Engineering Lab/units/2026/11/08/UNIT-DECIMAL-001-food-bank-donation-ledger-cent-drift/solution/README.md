# Reference — chỉ xem sau khi tự verify

Sai lệch xuất phát từ hai khác biệt so với hợp đồng: dùng `double` cho số thập phân tài chính và làm tròn **sau** aggregate với `ToEven`, thay vì làm tròn **từng** giao dịch với `AwayFromZero`. Dữ liệu 0.005 x3 phải là 0.01 x3 = 0.03, không phải 0.02.

Thay nội dung `starter/Program.cs` bằng nội dung [reference](Program.cs), chạy lại `./verify.ps1`. Không copy solution vào đường dẫn mà verify không kiểm tra.

Wrong fixes: (1) đổi `ToEven` thành `AwayFromZero` nhưng chỉ round tổng vẫn ra 0.02; (2) đổi `double` sang `decimal` nhưng round tổng vẫn sai; (3) tăng format lên F3 chỉ che sai khác; (4) sửa expected tests làm mất bảo vệ contract.

Trade-off: quyết định round-per-transaction phải do nghiệp vụ chốt. Với chính sách round-at-invoice-level, kết quả đúng sẽ khác; không tự áp chính sách này cho mọi hệ thống tài chính.
