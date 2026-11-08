# UNIT-MUTKEY-001 — Phiếu mượn dụng cụ biến mất khỏi bảng tra cứu

## Mục tiêu
Điều tra tính đúng đắn của cấu trúc tra cứu trong .NET khi dữ liệu nghiệp vụ được chỉnh sửa; chứng minh fix bằng regression test.

## Bối cảnh thực tế
Trung tâm cho mượn dụng cụ cộng đồng dùng một bảng tra cứu in-memory để tìm phiếu giữ chỗ. Sau khi nhân viên sửa mã hiển thị của một dụng cụ, phiếu vẫn có trong báo cáo nhưng truy vấn tra cứu trả về không tìm thấy.

## Bạn cần làm gì
Chạy starter, ghi nhận số lượng phiếu và kết quả tra cứu trước/sau thao tác cập nhật. Đưa ít nhất hai giả thuyết; sửa starter để phiếu luôn tìm được bằng mã định danh ổn định mà không phụ thuộc thay đổi tên/mã hiển thị.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7+ (hoặc Windows PowerShell 5.1). Không cần dịch vụ ngoài.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/run.ps1
```

## Cách reproduce vấn đề
```powershell
./scripts/reproduce.ps1
```
Lệnh chạy phiên bản starter với phép kiểm tra tái hiện và phải trả mã thoát khác 0.

## Những gì cần quan sát
Ghi lại số phần tử, kết quả tìm kiếm trước/sau cập nhật và mã thoát. Phân biệt mất dữ liệu thật với lỗi chỉ mục/tra cứu.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong `workspace/my-investigation.md`.
3. Thử fix tại `starter/Program.cs`.
4. Chạy `./scripts/verify.ps1` trên chính starter đã sửa.
5. Chỉ sau đó xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải tham khảo](solution/README.md).

## Expected Results
Ban đầu: có 1 phiếu nhưng lookup sau cập nhật thất bại. Sau fix: cả lookup trước và sau cập nhật đều thành công; số phiếu vẫn bằng 1.

## Estimated Time
30–40 phút (L1, D4).
