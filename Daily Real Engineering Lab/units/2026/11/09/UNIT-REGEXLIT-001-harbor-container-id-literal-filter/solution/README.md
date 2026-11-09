# Reference Solution — spoiler

## Symptoms
Nhãn chứa dấu đặc biệt cho kết quả không chính xác hoặc không tìm thấy.

## Evidence
`BX.04` có thể match `BXZ04`; `P+7` bị diễn giải khác chuỗi literal.

## Root cause
`Regex.IsMatch(input, pattern)` diễn giải chuỗi người dùng quét thành regular expression, cho phép meta-characters và substring match.

## Why fix works
Dùng `string.Equals(label, scannedLabel, StringComparison.Ordinal)` để bảo toàn exact, case-sensitive literal semantics.

## Verify
Copy `solution/ContainerLookup.cs` sang `starter/ContainerLookup.cs`, chạy `pwsh ./scripts/verify.ps1`.

## Alternative fixes
Nếu thật sự cần regex, phải tách API, kiểm tra cú pháp, giới hạn timeout, escape và anchor theo contract. Nếu nghiệp vụ cần case-insensitive, chọn comparer có chủ đích.

## Wrong fixes
- Chỉ `Regex.Escape` nhưng không anchor: substring false-positive.
- Catch exception: không xử lý silent false-positive.
- Xóa ký tự đặc biệt: phá identifier hợp lệ.

## Production implications
Giới hạn độ dài input, audit mismatches và đo tỷ lệ false-positive; nếu tập mã lớn, cân nhắc `HashSet<string>` với comparer chính xác.

## Senior perspective & trade-offs
Bảo vệ contract bằng regression fixtures trước refactor; rollout có monitoring và rollback. Đừng thêm caching hoặc scale-out khi lỗi thuộc semantics.
