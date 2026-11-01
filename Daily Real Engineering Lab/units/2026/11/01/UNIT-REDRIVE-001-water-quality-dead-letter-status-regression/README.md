# UNIT-REDRIVE-001 — Trạng thái điểm kiểm nghiệm nước bị đảo ngược sau khi xử lý dead-letter

## Mục tiêu
Tái hiện một sự cố trạng thái công khai không khớp với nguồn phát sự kiện; thu thập bằng chứng, sửa projection trong C# và đưa ra quyết định kiến trúc để hệ thống chịu được replay, duplicate delivery và crash.

## Bối cảnh thực tế
Một nền tảng công bố trạng thái các điểm kiểm nghiệm nước uống cho người dân. Hệ thống nhận thông báo từ phòng thí nghiệm qua broker; một số thông báo được xử lý lại từ dead-letter queue sau sự cố tích hợp. Dashboard đôi khi hiển thị trạng thái cũ dù có bản cập nhật mới hơn. Nhóm vận hành không được xóa sự kiện, dừng nhận dữ liệu hoặc sửa thủ công trạng thái để che triệu chứng.

## Bạn cần làm gì
1. Chạy mô phỏng và contract harness, ghi lại kết quả và timeline.
2. Phân biệt giả thuyết: dữ liệu nguồn sai, cache đọc cũ, sự cố delivery hay lỗi cập nhật projection.
3. Chỉnh sửa **starter/StatusProjection.cs** để các bất biến nghiệp vụ trong contract được bảo vệ.
4. Chạy verify trên chính starter đã sửa.
5. Viết một ADR ngắn trong workspace: điều gì phải được lưu bền vững, transaction boundary, cách replay DLQ an toàn, cách xử lý sự kiện thiếu phiên bản.
6. Chỉ sau khi tự làm, mở reference solution.

## Yêu cầu môi trường
.NET SDK 8.0.x, PowerShell 7 hoặc Windows PowerShell 5.1. Không cần Azure, Docker, database hay NuGet bên thứ ba. Kiểm tra `dotnet --version`; `global.json` cố định dòng SDK 8.0 (cho phép feature-band update).

## Chạy nhanh
Từ thư mục unit:
```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
# Sau khi tự sửa starter/StatusProjection.cs
./scripts/verify.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chạy các contract trên **starter gốc** và chỉ thành công khi các vi phạm dự kiến xuất hiện. Script này không dùng để chứng minh fix đã đúng. `verify.ps1` chạy cùng contract với tiêu chí ngược lại: không còn vi phạm.

## Những gì cần quan sát
So sánh log ứng dụng, `evidence/incident-timeline.md`, các trạng thái theo từng điểm và số lần áp dụng thay đổi. Đặc biệt quan sát khi cùng một message đến nhiều lần hoặc một thông báo cũ xuất hiện muộn.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis và bằng chứng.
3. Thử fix trong starter.
4. Verify cả tính đúng đắn lẫn bất biến xử lý message.
5. Mở solution sau cùng.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp và trade-offs](solution/README.md) · [Mã tham khảo](solution/StatusProjection.cs)

## Expected Results
[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md)

## Estimated Time
65 phút (35 phút điều tra và sửa, 30 phút quyết định kiến trúc).
