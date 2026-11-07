# Reference Solution — UNIT-UTF8CHUNK-001

**Chỉ đọc sau khi đã reproduce, viết hypothesis và tự sửa.**

## Symptoms
Ghi chú có dấu xuất hiện U+FFFD khi transport chia message thành các chunk; bản ghi ASCII bình thường. Byte sequence lỗi cũng được lưu thay vì bị từ chối.

## Evidence
`one-byte` và `mixed` nhận cùng byte stream hợp lệ nhưng cho kết quả sai; `ascii` đúng. `invalid-sequence` và `truncated-sequence` không ném lỗi. Điều này loại trừ lỗi riêng của storage và chỉ ra sự phụ thuộc vào cách chia đoạn.

## Root cause
Starter gọi `Encoding.UTF8.GetString(fragment.Span)` độc lập trên mỗi fragment. Một UTF-8 code point có thể trải qua hai fragment. Lần gọi độc lập không giữ byte đầu cho lần kế tiếp, vì vậy replacement fallback sinh U+FFFD. Mặc định fallback cũng chấp nhận dữ liệu không hợp lệ.

## Why the fix works
[Reference code](ChunkedNoteDecoder.cs) dùng `UTF8Encoding(false, true).GetDecoder()` một lần cho cả message. Decoder giữ trạng thái giữa các fragment; `flush: false` trong vòng lặp và `flush: true` đúng cuối thông điệp. Strict fallback ném `DecoderFallbackException` khi gặp byte lỗi hoặc tail thiếu byte.

## Cách áp dụng và verify
```powershell
Copy-Item ./solution/ChunkedNoteDecoder.cs ./starter/ChunkedNoteDecoder.cs -Force
./verify.ps1
```

`verify.ps1` chạy learner-editable starter, không chạy riêng reference. Kiểm tra cả ba valid cases và hai invalid cases. Không thay `starter/Program.cs`.

## Alternative fixes
1. Ghép toàn bộ byte vào buffer có giới hạn rồi decode **một lần** với `new UTF8Encoding(false, true)`: đơn giản, đúng cho message nhỏ, nhưng tăng memory peak.
2. Dùng `Decoder.Convert` và một output buffer tái sử dụng: thích hợp stream dài, cần quản lý `bytesUsed`, `charsUsed`, `completed` và flush cẩn thận.
3. Dùng `StreamReader` với strict UTF-8 và luồng dữ liệu thực: thuận tiện khi có stream owner rõ ràng; phải xác định đúng message boundary.

## Wrong / tempting fixes
- `Trim()` hoặc xóa U+FFFD: làm mất dữ liệu, không khôi phục byte gốc.
- Thay UTF-8 bằng ASCII: có thể làm test ASCII xanh nhưng phá yêu cầu tiếng Việt.
- Tăng kích thước read buffer: chỉ giảm xác suất cắt giữa code point, không bảo đảm đúng.
- Chỉ bật strict fallback trên mỗi fragment: sẽ từ chối payload hợp lệ bị cắt giữa code point.
- Chỉ fix happy path và chấp nhận replacement trên malformed payload: che giấu corruption.

## Production implications và trade-offs
- Giới hạn số byte/message, số fragment và tổng thời gian nhận; tránh giữ payload không giới hạn.
- Khi streaming, xác định rõ thời điểm kết thúc message và không reuse decoder qua các message độc lập.
- Ghi metrics `invalid_utf8_count`, `fragment_count`, `decode_failure`; không log toàn bộ ghi chú khách hàng.
- Nếu protocol cho phép byte không hợp lệ, phải định nghĩa chính sách rõ ràng thay vì mặc định chấp nhận dữ liệu biến dạng.
- Cân nhắc cancellation và buffer ownership nếu source là socket hoặc pipeline.

## Senior engineering judgment
Bất biến cần bảo vệ là `Decode(Concatenate(chunks)) == Decode(originalBytes)` với payload hợp lệ, **bất kể chunk boundaries**. Kiểm tra property này và invalid payload ở biên là mạnh hơn một test với một chunk cố định. Không đổ lỗi cho transport chỉ vì kích thước chunk thay đổi.
