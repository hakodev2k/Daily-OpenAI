# UNIT-STREAMPOS-001 — Ảnh số hóa có checksum đúng nhưng object rỗng

## Mục tiêu
Điều tra hành vi Stream và refactor pipeline upload mà không phá checksum.

## Bối cảnh thực tế
Bảo tàng số hóa ảnh. Manifest chứa SHA-256 đúng nhưng object storage nhận 0 byte sau khi triển khai bản nâng cấp.

## Bạn cần làm gì
Reproduce; ghi evidence tại mỗi bước I/O; đưa ra hai hypothesis; sửa starter và chạy verify.

## Yêu cầu môi trường
.NET SDK 8 và PowerShell 7, không cần external service.

## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` và đối chiếu SOURCE_BYTES, STORED_BYTES, SHA256, SOURCE_POSITION.

## Những gì cần quan sát
Nguồn có dữ liệu và checksum được tạo, nhưng số byte đã lưu không đúng.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó xem solution.

## Hints
[Hint 1](hints/01.md) · [Hint 2](hints/02.md) · [Hint 3](hints/03.md)

## Reference Solution
**Spoiler:** [Lời giải](solution/README.md)

## Expected Results
Object bytes giống hệt source, checksum không thay đổi, verify trả exit 0. Hành vi với non-seekable stream được xác định.

## Estimated Time
50 phút.
