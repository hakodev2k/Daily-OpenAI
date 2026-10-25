# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms
Import có thể dừng tại một SKU gần hợp lệ; những SKU sai rõ ràng vẫn được từ chối bình thường. Bản gốc có thể ném RegexMatchTimeoutException.

## 2. Evidence
Starter dùng một pattern có nhiều cách phân nhóm cho cùng một chuỗi ký tự. Trường hợp 512 ký tự A và dấu ! khiến engine cố các khả năng trước khi kết luận không match. Bộ test tách valid/invalid/near-match và báo exception.

## 3. Root cause
Pattern gốc ^([A-Z0-9]+)+$ lồng hai lượng từ +, khiến backtracking có số nhánh tăng rất nhanh với near-match; timeout chỉ giới hạn thời gian mỗi lần kiểm tra, không loại bỏ chi phí.

## 4. Why the fix works
Thay bằng grammar tuyến tính \A[A-Z0-9]+\z với RegexOptions.NonBacktracking, dùng regex cache tĩnh và vẫn giới hạn chiều dài. Không có cách phân chia nhóm lồng nhau; anchor buộc khớp toàn bộ chuỗi.

## 5. How to verify
Chỉ sửa starter/SkuValidator.cs, chạy ./verify.ps1. Tất cả 10 case phải PASS. Có thể chạy thêm dotnet run --project ./solution/RegexLab.csproj -- --verify để kiểm tra riêng reference, nhưng không thay thế kiểm tra starter.

## 6. Alternative fixes
Duyệt từng char bằng vòng for và kiểm tra phạm vi A–Z/0–9, O(n), không cấp phát Regex; phù hợp grammar đơn giản. Một regex không nested quantifier với timeout hợp lý cũng có thể đủ. RegexOptions.NonBacktracking chỉ phù hợp các pattern được engine này hỗ trợ.

## 7. Wrong / tempting fixes
- Tăng timeout từ 100 ms lên vài giây: làm chậm batch lâu hơn khi gặp dữ liệu xấu.
- Bắt exception rồi trả false: tránh crash nhưng vẫn tiêu tốn ngân sách CPU; có thể dùng như hàng rào phòng vệ, không phải sửa nguyên nhân.
- Tắt timeout: có thể làm worker kẹt lâu hơn.
- Chặn mọi SKU dài: có thể giảm rủi ro nhưng vi phạm contract 1–2048 nếu tự ý hạ giới hạn.
- Scale out trước khi xác định chi phí mỗi validation: chỉ nhân chi phí và rủi ro.

## 8. Production implications
Giới hạn kích thước request/batch, rate limit nhà cung cấp, metrics cho validation timeout và độ dài input (không log SKU nhạy cảm nguyên văn), xử lý bản ghi lỗi có kiểm soát thay vì làm thất bại toàn batch nếu business cho phép.

## 9. Trade-offs
Regex tuyến tính dễ diễn đạt grammar và bảo trì; vòng lặp ký tự đơn giản hơn và tránh phụ thuộc regex engine. NonBacktracking không hỗ trợ một số tính năng như backreferences, nên phải kiểm tra tương thích khi grammar mở rộng.

## 10. What a Senior engineer should notice
Timeout là triệu chứng và là safety guard, không phải bằng chứng về root cause. Cần tái hiện bằng dữ liệu có kiểm soát, phân biệt semantic validation với operational failure, xác nhận độ phức tạp, giữ nguyên contract, và bổ sung regression case near-match.

## Tự đối chiếu
File tham khảo: [SkuValidator.cs](SkuValidator.cs). Project solution chỉ phục vụ so sánh; ./verify.ps1 luôn trỏ vào starter.

## Tài liệu chính thức
- Microsoft Learn: https://learn.microsoft.com/dotnet/standard/base-types/backtracking-in-regular-expressions
- Microsoft Learn: https://learn.microsoft.com/dotnet/standard/base-types/best-practices-regex
