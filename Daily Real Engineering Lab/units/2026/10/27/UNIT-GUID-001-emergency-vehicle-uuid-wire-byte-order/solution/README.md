# Reference Solution — spoiler

Chỉ đọc sau khi đã reproduce, thu thập evidence và tự sửa starter.

## Symptoms
Local Guid round-trip xanh, nhưng đối tác lookup unknown vehicle. Outbound frame khác golden vector và inbound frame decode ra ID khác.

## Evidence
EVT-A yêu cầu 00112233445566778899AABBCCDDEEFF, nhưng starter xuất 33221100554477668899AABBCCDDEEFF. Sự khác biệt tập trung ở các trường đầu; internal round-trip dùng cùng quy ước nên che giấu lỗi.

## Root cause
Guid.ToByteArray() và new Guid(ReadOnlySpan<byte>) mặc định dùng mixed-endian representation cho các trường đầu. Partner định nghĩa UUID theo network byte order, nên 16 bytes khác nhau dù textual ID giống nhau.

## Why the fix works
.NET 8 hỗ trợ bigEndian: true cho cả encode và decode. Reference implementation trong VehicleEventCodec.cs áp dụng explicit wire order ở cả hai chiều, giữ nguyên length guard.

## How to verify
Chạy ./verify.ps1, yêu cầu sáu PASS. Local round-trip là điều kiện cần, không đủ; golden vectors từ bên ngoài mới kiểm tra interoperability.

## Alternative fixes
Dùng canonical 36-character UUID string nếu protocol cho phép, hoặc codec explicit field conversion cho runtime cũ. Không âm thầm đổi wire contract hiện hữu.

## Wrong / tempting fixes
- Chỉ sửa encode hoặc chỉ sửa decode: chiều còn lại vẫn sai.
- Dùng Guid.ToString() rồi chuyển UTF-8: tạo 36 bytes, phá binary frame contract.
- Chỉ kiểm tra local round-trip: chính starter cũng pass.
- Retry, tăng timeout hoặc đổi database index: không giải quyết mismatch xác định.

## Production implications
Thống nhất protocol spec, thêm golden vectors cross-language vào CI, kiểm kê format của các ID đã lưu. Nếu dữ liệu cũ theo mixed-endian, cần migration/dual-read có version rõ ràng, tránh reinterpret bừa bãi.

## Trade-offs
Binary 16 bytes gọn nhưng dễ sai representation; canonical text dễ debug hơn nhưng tốn băng thông. API explicit bigEndian của .NET 8 giảm code custom nhưng yêu cầu runtime tương thích.

## Senior engineering insight
Hai hàm đối xứng có cùng lỗi sẽ qua unit test round-trip. Contract integration cần fixtures độc lập và kiểm thử cả hai chiều cùng rollout plan.
