# Reference Solution — SPOILER

Chỉ đọc sau khi tự reproduce, điều tra và thử fix.

## 1. Symptoms
Gateway nhận frame hợp lệ nhưng map tới gate không tồn tại và đếm sai lượt vào.

## 2. Evidence
Frame A chứa field bytes `00 2A` và `03 E8`. Vendor kỳ vọng 42/1000, gateway báo 10752/59395. Frame B cho thấy cùng kiểu sai lệch; độ dài, signature và offsets không bất thường.

## 3. Root cause
Vendor dùng network byte order (**big-endian**). Starter lại giải mã bằng `BinaryPrimitives.ReadUInt16LittleEndian`. Vì vậy `00 2A` thành `0x2A00` thay vì `0x002A`.

## 4. Why the fix works
Đọc cả hai field bằng `BinaryPrimitives.ReadUInt16BigEndian` như trong [GateFrameDecoder.cs](GateFrameDecoder.cs). Đây là API tường minh, không phụ thuộc endianness của host.

## 5. How to verify
Sửa `starter/GateFrameDecoder.cs` rồi chạy `./verify.ps1`. Cả ba valid fixtures và bốn negative fixtures phải pass. `./reproduce.ps1` sau fix trả 1 vì lỗi cũ đã hết.

## 6. Alternative fixes
Có thể ghép byte bằng shifts hoặc reverse-endianness có kiểm soát, nhưng direct BigEndian API dễ review hơn.

## 7. Wrong / tempting fixes
- Sửa raw payload trước khi lưu: phá contract với các consumer khác.
- Chia entry count theo một hệ số: không đúng cho mọi giá trị.
- Chặn count lớn: chỉ giấu lỗi, có thể loại dữ liệu hợp lệ.
- Dùng `BitConverter.ToUInt16` không xử lý byte order: phụ thuộc CPU host.

## 8. Production implications
Gắn protocol version với decoder; giữ vendor fixtures độc lập, kiểm tra malformed frame và giới hạn sampling raw hex để tránh log quá nhiều.

## 9. Trade-offs
Parser thủ công ít dependency nhưng phải duy trì fixtures theo từng version. Chỉ thêm framework parser khi độ phức tạp wire contract thực sự cần.

## 10. Senior engineer should notice
Đây là **integration contract mismatch**, không phải cảm biến hỏng. Test byte-order cần giá trị bất đối xứng; `FF FF` không phân biệt được hai cách đọc.
