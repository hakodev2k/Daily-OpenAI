# Reference Solution — chỉ xem sau khi đã reproduce và thử sửa

## 1. Symptoms
Ba lần poll cấu hình logic giống nhau khiến gateway ghi nhận ba lệnh gửi xuống controller. Một lần sửa trực tiếp dãy ngưỡng đầu vào lại có thể không được nhận diện nếu object được tái sử dụng.

## 2. Evidence
Trong `harness/Program.cs`, trường hợp tạo ba `CalibrationProfile` với cùng `ZoneId`, `Revision`, dãy `MoistureThresholds` cho `AppliedZones.Count == 3` ở starter. Hai test về mutation cũng thất bại theo hướng ngược nhau.

## 3. Root cause
`record` tự sinh value equality theo từng property, nhưng `int[]` là reference type và `EqualityComparer<int[]>.Default` không so sánh nội dung phần tử. Hai array được cấp phát riêng có cùng số vẫn bị coi là khác nhau. Ngược lại, việc lưu nguyên reference đến array do caller sở hữu khiến snapshot đã áp dụng thay đổi ngầm khi caller sửa array.

## 4. Why the fix works
File [CalibrationSync.cs](CalibrationSync.cs) so sánh `Revision` và nội dung array bằng `AsSpan().SequenceEqual(...)`; khi lưu trạng thái đã áp dụng, nó `Clone()` array. Nhờ vậy, phép so sánh dựa trên giá trị nghiệp vụ, còn cache giữ snapshot độc lập với bộ nhớ mutable của caller.

## 5. How to verify
Sửa **starter/CalibrationSync.cs** theo cách riêng, sau đó chạy `./verify.ps1`. Cả 9 case phải PASS, bao gồm no-op với array mới cùng nội dung, update khi revision/ngưỡng đổi, nhiều zone, và mutation của input. `./reproduce.ps1` dùng cho trạng thái lỗi ban đầu; sau khi sửa, script này không còn tái hiện được lỗi nên có thể trả exit code 1. Có thể kiểm tra source tham khảo riêng bằng `dotnet build solution/GreenhouseLab.csproj -c Release`.

## 6. Alternative fixes
- Đổi API sang một immutable value object có equality theo từng ngưỡng; phù hợp khi kiểu dữ liệu được dùng rộng khắp và cần invariant mạnh.
- Tính fingerprint ổn định của nội dung cấu hình; hữu ích nếu dữ liệu lớn hoặc so sánh thường xuyên, nhưng cần xử lý collision/canonicalization và lifecycle rõ ràng.
- So sánh từng trường trực tiếp như reference solution; đơn giản, dễ audit cho payload nhỏ.

## 7. Wrong / tempting fixes
- Bỏ qua mọi poll có cùng `Revision`: có thể che mất thay đổi ngưỡng nếu upstream không tuân thủ versioning.
- Dùng `ReferenceEquals` hoặc giữ nguyên array từ lần poll trước: tạo false positive/false negative khi input được cấp phát lại hoặc mutate.
- Debounce bằng timer: chỉ che bớt lệnh thừa, không sửa contract so sánh.
- Serialize JSON để so sánh mọi lần: có thể đúng nếu canonical hóa chặt, nhưng tạo allocation và ràng buộc không cần thiết cho array nhỏ.

## 8. Production implications
Lệnh cấu hình dư thừa gây traffic, wear trên thiết bị và nhiễu audit log. Bỏ sót thay đổi có thể làm lịch tưới sai. Khi gọi thiết bị thật, cần thêm idempotency key/acknowledgment, timeout, retry có kiểm soát và telemetry theo zone; lab chỉ mô phỏng ranh giới quyết định gửi.

## 9. Trade-offs
`SequenceEqual` tốn O(n) mỗi lần so sánh và `Clone` tốn O(n) mỗi lần thay đổi; phù hợp dãy ngưỡng nhỏ. Với cấu hình lớn, cân nhắc immutable snapshot, version được kiểm chứng hoặc content hash có kiểm tra collision. Tránh tối ưu trước khi đo tần suất poll và kích thước payload.

## 10. What a Senior engineer should notice
Record không biến mọi object graph thành immutable hoặc deep-value-comparable. Cần xác định ai sở hữu memory, trạng thái đã áp dụng là snapshot hay live reference, và contract của revision có đáng tin cậy không. Test phải chứng minh cả hai phía: không gửi thừa và không bỏ sót thay đổi thật.
