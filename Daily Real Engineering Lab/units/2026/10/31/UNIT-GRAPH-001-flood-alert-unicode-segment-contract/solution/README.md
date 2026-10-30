# Reference Solution — chỉ xem sau khi tự sửa

## 1. Symptoms
Thông báo ASCII chạy đúng nhưng Unicode có segment bị gateway từ chối, hoặc xuất hiện ký tự hỏng sau khi gửi từng segment.

## 2. Evidence
`contract/Program.cs` đo từng segment: UTF-8 byte count, tính hợp lệ của surrogate, ranh giới grapheme, khả năng ghép nguyên văn, và hành vi khi element vượt budget. Case `ABCDe` + dấu kết hợp và case emoji tách được hai lỗi khác nhau.

## 3. Root cause
`string.Length` và `Substring` thao tác trên UTF-16 code units. Code ban đầu coi `maxBytes` như số `char` và cho phép cắt giữa surrogate pair hoặc base letter và combining mark. Số code unit không tương đương số byte UTF-8.

## 4. Why the fix works
`solution/MessageSplitter.cs` duyệt từng grapheme cluster bằng `StringInfo.GetTextElementEnumerator`, tính byte bằng `UTF8Encoding(false, true)` rồi gom phần mà không vượt budget. Không thể gửi một grapheme vượt budget mà vẫn giữ nguyên contract, nên từ chối có kiểm soát.

## 5. How to verify
Sau khi tự sửa `starter/MessageSplitter.cs`, chạy `./scripts/verify.ps1`. Có thể kiểm tra lời giải riêng bằng `dotnet run --project ./solution/Solution.csproj -- verify` từ thư mục unit. Không thay harness để làm xanh test.

## 6. Alternative fixes
- Nếu đối tác hỗ trợ UCS-2/SMS segment limit khác, thay byte-budget policy theo **contract thực tế**; không dùng quy tắc UTF-8 mặc định cho mọi SMS.
- Với dữ liệu chỉ ASCII được xác nhận và kiểm soát đầu vào, chia theo byte có thể đơn giản hơn nhưng phải kiểm soát validation.
- Có thể streaming grapheme qua `StringInfo` để giảm memory cho payload lớn; cân nhắc giới hạn đầu vào trước.

## 7. Wrong or misleading fixes
- `Substring` cộng thêm một ký tự khi gặp emoji: không xử lý mọi combining sequence, byte budget và biến thể Unicode.
- `Encoding.UTF8.GetBytes` rồi cắt mảng: có thể cắt giữa UTF-8 sequence hoặc grapheme.
- Chỉ kiểm tra `string.Concat(parts) == original`: ghép lại đúng vẫn không bảo đảm từng phần có thể gửi được.
- Tăng giới hạn gateway trong config mà không xác nhận hợp đồng: che triệu chứng và tăng nguy cơ bị từ chối.

## 8. Production implications
Đo tỷ lệ từ chối theo loại payload, giới hạn kích thước input, không log toàn bộ nội dung cảnh báo có thông tin cá nhân. Xác nhận thứ tự segment, tính idempotent khi retry và cách đối tác đánh số segment.

## 9. Trade-offs
Grapheme-aware segmentation phức tạp và tốn CPU hơn char slicing nhưng giữ toàn vẹn hiển thị. Quy tắc từ chối grapheme quá lớn có thể cần fallback nghiệp vụ (rút gọn nội dung, gửi kênh khác), không âm thầm phá dữ liệu.

## 10. What a Senior engineer should notice
Giới hạn byte, giới hạn code point và giới hạn grapheme là ba contract khác nhau. Thử nghiệm ASCII không chứng minh được tính đúng đắn cho nội dung đa ngôn ngữ. Đặc biệt, SMS thực tế thường dùng GSM-7/UCS-2 chứ không mặc nhiên UTF-8: ở đây UTF-8 là contract giả lập của gateway để luyện kỹ năng, phải xác minh trước khi triển khai thật.

## Official references
- [Microsoft: StringInfo](https://learn.microsoft.com/en-us/dotnet/api/system.globalization.stringinfo)
- [Microsoft: UTF8Encoding](https://learn.microsoft.com/en-us/dotnet/api/system.text.utf8encoding)
- [Microsoft: .NET character encoding](https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-encoding-introduction)
