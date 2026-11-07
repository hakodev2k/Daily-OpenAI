# Investigation guide
1. Chạy `reproduce.ps1`; lưu exit code và hai dòng kết quả.
2. Kiểm tra input XML trong `starter/Program.cs` và output của `ReceiptXmlParser.Parse`.
3. Hypothesis A: đối tác thực sự gửi nội dung ghi chú. Hypothesis B: thành phần parse bổ sung dữ liệu. Hypothesis C: logging làm lẫn nội dung giữa các phiếu.
4. Tìm một phép thử phân biệt từng hypothesis mà không cần đọc file bí mật thật.
5. Kiểm tra đường đi của XML từ `StringReader` đến `XDocument`; ghi rõ trust boundary.
6. Chỉ sửa parser. Chạy `verify.ps1`; nếu chưa pass, xem exception và điều chỉnh.
7. Đánh giá tác động đến đối tác hợp lệ có DTD, backward compatibility và chính sách từ chối payload.

Không copy `solution/` trước khi tự thử. File canary là fixture vô hại; không dùng secret thật.
