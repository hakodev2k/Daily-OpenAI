# Reference Solution — chỉ đọc sau khi tự reproduce và thử sửa

## 1. Symptoms
Một ID gần đúng nhưng có ký tự sai ở cuối khiến worker ném `RegexMatchTimeoutException`; các ID bình thường đi qua. Đây là lỗi khả dụng của bước validation, không phải kết quả nghiệp vụ `false`.

## 2. Evidence
Với input 48 chữ `A` rồi `!`, biểu thức của starter có thể thử rất nhiều cách phân chia các ký tự hợp lệ trước khi kết luận thất bại. ID hợp lệ dài đi qua nhanh hơn đáng kể. Bộ regression tách case thường, case dài hợp lệ và case dài không hợp lệ.

## 3. Root cause
`([A-Z0-9]+)+` chứa quantifier lồng nhau, tạo nhiều phân hoạch tương đương khi engine backtracking tìm cách khớp phần cuối. Khi chuỗi gần khớp nhưng thất bại ở cuối, số nhánh có thể tăng theo cấp số nhân. Timeout giới hạn tác hại nhưng không biến timeout thành một câu trả lời validation hợp lệ.

## 4. Why the fix works
Grammar chỉ cần một lần lặp: `\A[A-Z0-9]+\z`. Loại bỏ cấu trúc mơ hồ, đồng thời `RegexOptions.NonBacktracking` trong .NET 8 cung cấp bảo đảm thời gian tuyến tính cho biểu thức tương thích. Giữ timeout hữu hạn làm defense-in-depth. Xem [ReferenceMeterCodeValidator.cs](ReferenceMeterCodeValidator.cs); sao chép ý tưởng sang file learner, không đổi `Program.cs`.

## 5. How to verify
Chỉnh `starter/MeterCodeValidator.cs`, chạy `./verify.ps1`. Phải có `VERIFICATION_PASSED` và exit code 0; kiểm tra đủ năm case, đặc biệt chuỗi 48 chữ `A` kết thúc bằng `!`. Chạy `./reproduce.ps1` trước khi sửa để thu thập baseline; sau khi sửa script reproduce không còn cần pass.

## 6. Alternative fixes
- Dùng vòng lặp `foreach (char c in code)` với kiểm tra `A–Z`, `0–9` và non-empty: đơn giản, tuyến tính, không phụ thuộc Regex.
- Dùng regex một quantifier duy nhất và timeout hữu hạn; có thể không cần NonBacktracking với grammar đơn giản này, nhưng engine tuyến tính giảm rủi ro khi grammar mở rộng.

## 7. Wrong / tempting fixes
- Tăng timeout lên vài giây: chuyển tải vấn đề sang latency và capacity; không loại bỏ số nhánh.
- Bỏ timeout: có thể làm worker bị giữ vô hạn bởi input đối nghịch.
- Catch `RegexMatchTimeoutException` rồi coi ID là hợp lệ: sai fail-closed contract.
- Bỏ case dài khỏi test: che lỗi mà không sửa cơ chế.
- Scale out worker: tăng chi phí mà mỗi worker vẫn dễ bị một input gây nghẽn.

## 8. Production implications
Giới hạn kích thước input ở ingress, giới hạn số ID/batch, log loại lỗi và kích thước thay vì toàn bộ ID nhạy cảm; theo dõi tỷ lệ timeout và hàng đợi. Không dựa duy nhất vào regex timeout để bảo vệ CPU khi có nhiều request đồng thời.

## 9. Trade-offs
Vòng lặp ASCII rất dễ audit và ổn định nhưng kém linh hoạt khi grammar tăng. Regex ngắn gọn, dễ mở rộng nhưng phải review complexity, anchors và engine compatibility. `RegexOptions.NonBacktracking` không hỗ trợ mọi tính năng regex như backreferences, nên cần test khi thay đổi grammar.

## 10. Senior engineer insight
Validation là một ranh giới tin cậy và tài nguyên: dữ liệu không hợp lệ phải được từ chối với chi phí hữu hạn, không được biến thành sự cố availability. Đo worst-case gần khớp, không chỉ happy path.
