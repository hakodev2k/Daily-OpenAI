# Reference Solution — UNIT-MUTDICT-001

## Root cause
`ParcelKey` là record dùng value equality; các property tham gia `GetHashCode()`. `Dictionary` đặt entry vào bucket theo hash tại thời điểm insert. Sau khi đổi `Route`, hash mới khác hash lúc insert, vì vậy tra cứu có thể thất bại ngay cả với cùng object. `Count` không phản ánh khả năng lookup.

## Fix
Dùng định danh bất biến `ParcelId` (string, `StringComparer.Ordinal`) làm khóa, còn `Route` thuộc giá trị có thể thay thế. Không cần quét toàn bộ manifest, không cần tái tạo dictionary sau mỗi lần sửa tuyến.

## Chạy reference
`dotnet run --project ./solution/ParcelLab.csproj` phải in `PASS` và exit 0.

## Wrong fixes
- Duyệt `manifest.Keys` mỗi lần tra cứu: che triệu chứng, làm mất lợi thế hash lookup.
- Đổi `Route` ngược lại trước lookup: không giữ đúng dữ liệu nghiệp vụ.
- Tạo key mới từ route hiện tại: hash vẫn không trỏ về bucket ban đầu.
- Chỉ tăng capacity: không thay đổi hợp đồng hash/equality.

## Trade-offs
Nếu dùng composite key thực sự, phải tạo key bất biến và remove/insert một cách nhất quán khi thay đổi khóa. Với bài toán này route không phải identity, nên dùng ID ổn định đơn giản hơn.

## Verify
Chạy `./scripts/verify.ps1` trên starter đã sửa. Script không kiểm tra solution thay cho bài làm.
