# UNIT-LANGQ-001 — Caption API trả sai ngôn ngữ khi nhiều ưu tiên cùng xuất hiện

## Mục tiêu
Refactor một thành phần lựa chọn ngôn ngữ của API mà không phá hợp đồng với các ứng dụng đã triển khai. Tự dựng bằng chứng trước khi chỉnh sửa, sau đó kiểm tra các trường hợp biên.

## Bối cảnh thực tế
Hệ thống lưu trữ di sản cung cấp caption tiếng Việt, Anh và Pháp. Sau khi ứng dụng di động cập nhật cấu hình ngôn ngữ, một số người xem nhận caption không đúng ưu tiên, thậm chí có yêu cầu không chọn được ngôn ngữ phù hợp. Đội vận hành ghi nhận hiện tượng phụ thuộc vào chuỗi header do client gửi; API không bị lỗi 500.

## Bạn cần làm gì
1. Chạy baseline để xác nhận sự khác biệt giữa kết quả thực tế và hợp đồng.
2. Lập bảng bằng chứng cho ít nhất bốn request, nêu tối thiểu hai giả thuyết.
3. Chỉnh **duy nhất** `starter/CaptionLocale.cs` để vượt qua các hợp đồng mà không sửa harness.
4. Đánh giá hành vi khi không có header, không có lựa chọn phù hợp và khi client gửi giá trị không hợp lệ.

## Yêu cầu môi trường
- .NET SDK 8.x; không cần NuGet ngoài .NET SDK, Docker hoặc Azure.
- PowerShell 7+ trên Windows (có thể chạy các lệnh `dotnet` tương đương trên Linux/macOS).

## Chạy nhanh
Tại thư mục unit:
```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
`./scripts/reproduce.ps1` chạy một bản baseline **bất biến** và kiểm tra các triệu chứng đã biết. Exit code 0 nghĩa là vấn đề được tái hiện; không có nghĩa baseline đúng.

## Những gì cần quan sát
- Giá trị header nguyên văn, locale thực tế, locale được mong đợi và mức độ lệch.
- Request nào thành công và request nào bị lựa chọn sai.
- Hành vi của client có nhiều lựa chọn, có ngôn ngữ vùng và không có lựa chọn phù hợp.
- Dữ liệu bổ sung: [evidence/requests.md](evidence/requests.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong [workspace/my-investigation.md](workspace/my-investigation.md).
3. Thử fix trong `starter/`.
4. Verify bằng `./scripts/verify.ps1`; exit code 0 chỉ khi tất cả contract pass.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md) và [solution/CaptionLocale.cs](solution/CaptionLocale.cs) — chỉ mở sau khi tự điều tra.

## Expected Results
[Trước](expected-results/before.md) · [Sau](expected-results/after.md)

## Estimated Time
60–80 phút (L4 / D4).
