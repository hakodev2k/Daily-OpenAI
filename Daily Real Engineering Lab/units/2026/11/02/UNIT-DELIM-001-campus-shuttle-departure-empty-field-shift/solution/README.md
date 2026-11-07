# Reference Solution — chỉ đọc sau khi tự điều tra

## 1. Symptoms
Bản ghi có `note` rỗng bị chuyển `destination` sang `note`, `status` sang `destination` và trạng thái cuối thành `UNKNOWN`. Một số bản ghi thiếu hoặc thừa trường vẫn được parser chấp nhận.

## 2. Evidence
Baseline `--check` có `CONTRACT_FAIL` ở các trường hợp note rỗng và record không đúng hợp đồng. Bản ghi có đủ năm giá trị không rỗng vẫn pass, khiến smoke test đơn giản bỏ sót lỗi.

## 3. Root cause
`StringSplitOptions.RemoveEmptyEntries` loại bỏ phần tử rỗng **mang ý nghĩa vị trí** trong giao thức năm cột. Parser tiếp tục cho phép 4 hoặc 5 token và gán theo chỉ số, nên mất căn chỉnh mà không có exception.

## 4. Why the fix works
Giữ token rỗng khi split; yêu cầu **chính xác năm token**; cho phép `note` rỗng nhưng từ chối các trường bắt buộc rỗng. `solution/DepartureParser.cs` là một triển khai tham khảo, không phải mã mà `verify.ps1` chạy.

## 5. How to verify
```powershell
./scripts/reproduce.ps1       # baseline vẫn tái hiện lỗi
./scripts/verify.ps1          # starter sau khi bạn sửa phải có 7 CONTRACT_PASS
./scripts/verify-reference.ps1 # kiểm tra độc lập reference solution
```
Không sửa harness để làm test xanh.

## 6. Alternative fixes
Có thể dùng parser dạng state machine hoặc thư viện parsing với schema và số cột cố định khi giao thức sau này có escaping/quoting. Với hợp đồng đơn giản không có escaped pipe, `Split` giữ token rỗng và kiểm tra schema là phương án đủ rõ ràng.

## 7. Wrong / tempting fixes
- Thêm `UNKNOWN` cho trường thiếu chỉ che mất lỗi dữ liệu và tạo thông tin giả.
- Chèn một `note` rỗng ở cuối mảng sau khi split không phục hồi được vị trí trường đã mất.
- Bỏ case lỗi hoặc sửa expected output trong harness chỉ làm test xanh giả.
- Thêm Redis, retry hoặc queue không giải quyết được lỗi parsing thuần túy.

## 8. Production implications
Cần log số lượng record bị từ chối, lý do và mã đối tác (không log thông tin nhạy cảm); cách ly record sai schema thay vì phát thông tin sai lên màn hình. Giám sát tỷ lệ lỗi theo phiên bản vendor feed và có contract tests trong CI.

## 9. Trade-offs
Strict parsing có thể làm tăng số record bị từ chối khi vendor đổi schema. Đây là lựa chọn có chủ đích: lỗi hiển thị sai âm thầm nguy hiểm hơn lỗi bị từ chối có thể theo dõi. Nếu cần tương thích nhiều phiên bản, phân nhánh parser theo version rõ ràng.

## 10. Senior engineering observation
Phân biệt **syntax** và **semantics**: dữ liệu được parse không có nghĩa là đúng hợp đồng. Test phải bao phủ giá trị rỗng ở vị trí tùy chọn, thiếu/thừa cột và trường bắt buộc rỗng, không chỉ happy path.

## Source of truth
- [String.Split API](https://learn.microsoft.com/en-us/dotnet/api/system.string.split)
- [StringSplitOptions](https://learn.microsoft.com/en-us/dotnet/api/system.stringsplitoptions)
