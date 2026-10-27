# Reference Solution — chỉ đọc sau khi tự reproduce và thử fix

## 1. Symptoms
Feed có 6 quan sát nhưng client lấy theo trang chỉ nhận 5, mọi lần gọi đều thành công. Kích thước trang ảnh hưởng tới số bản ghi nhận được.

## 2. Evidence
Fixture có ba bản ghi `obs-a`, `obs-b`, `obs-c` cùng `ObservedAtUtc`. Với page size 2, trang đầu kết thúc tại `obs-b`; trang sau bắt đầu ở `obs-d`, bỏ qua `obs-c`.

## 3. Root cause
Thứ tự thực tế là `(ObservedAtUtc, ObservationId)`, nhưng cursor chỉ giữ `ObservedAtUtc` và filter dùng `timestamp > lastTimestamp`. Khi trang kết thúc giữa nhóm timestamp bằng nhau, mọi phần tử còn lại của nhóm bị loại.

## 4. Why the fix works
Cursor lưu cả UTC ticks và ID. Bộ lọc dùng cùng thứ tự từ điển với `OrderBy/ThenBy`: `t > cursor.t || (t == cursor.t && id > cursor.id)`. ID là khóa ổn định và duy nhất trong fixture. Base64 JSON chỉ là định dạng token, không phải biện pháp bảo mật.

## 5. How to verify
Sau khi tự làm, chạy `./verify.ps1`. Để đối chiếu reference, copy `solution/CursorPager.cs` vào `starter/CursorPager.cs`, chạy lại `./verify.ps1`, rồi khôi phục code của bạn nếu cần. `./reproduce.ps1` vẫn kiểm tra baseline.

## 6. Alternative fixes
- Offset pagination có thể hợp lý với dataset nhỏ và snapshot cố định nhưng dễ drift khi dữ liệu thay đổi.
- Dùng unique monotonic sequence key nếu hệ thống có khóa duy nhất và yêu cầu thứ tự phù hợp.
- Cursor có thể được ký HMAC nếu API cần chống sửa token, nhưng ký không thay thế tính đúng đắn của thứ tự.

## 7. Wrong or misleading fixes
- Tăng pageSize: có thể che triệu chứng ở fixture, không bảo đảm với batch lớn.
- Dùng `>= timestamp`: tránh bỏ sót một phần nhưng có thể trả trùng, thậm chí lặp trang.
- `Distinct()` ở client: không phục hồi được bản ghi đã bị bỏ qua.
- Thêm retry HTTP: request thành công về mặt transport, lỗi thuộc logic ranh giới.

## 8. Production implications
Bảo đảm `ObservationId` duy nhất và immutable, `ObservedAtUtc` được chuẩn hóa UTC. Nếu feed thay đổi trong lúc duyệt, cần xác định snapshot/high-water mark hoặc consistency contract; bài này cố ý giữ nguồn bất biến. Cần index phù hợp trên `(ObservedAtUtc, ObservationId)` ở DB thật.

## 9. Trade-offs
Composite cursor dài hơn timestamp, đổi contract token có thể cần versioning, và keyset không cho nhảy tới trang N trực tiếp. Bù lại nó xác định vị trí chính xác trong một thứ tự toàn phần.

## 10. Senior engineer should notice
Kiểm tra invariant `no gaps, no duplicates, stable order, eventual termination` trên nhiều page sizes; phân biệt lỗi logical pagination với network; xác định rõ uniqueness, mutability và snapshot semantics trước khi triển khai lên DB.

**Không có .NET runtime execution trong lần tạo lab này; reference đã được rà soát tĩnh, người học cần chạy build/tests local.**
