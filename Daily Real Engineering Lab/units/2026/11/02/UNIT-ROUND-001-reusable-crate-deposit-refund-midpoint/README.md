# UNIT-ROUND-001 — Tiền hoàn cọc thùng tái sử dụng lệch một xu

## Mục tiêu

Reproduce và điều tra sai lệch tiền hoàn cọc ở một batch thanh toán; đối chiếu kết quả với hợp đồng nghiệp vụ, đưa ra giả thuyết, sửa đường tính toán của chính mình và xác nhận các ca biên mà không làm sai giao dịch thông thường.

## Bối cảnh thực tế

Một hệ thống quản lý thùng vận chuyển tái sử dụng tính số tiền hoàn cọc cho cửa hàng khi thùng được trả về. Số lượng có dấu âm biểu thị giao dịch đảo bút toán. Báo cáo đối soát phát hiện một số khoản hoàn tiền chênh 0,01 đơn vị tiền tệ so với quy tắc đã ký với đối tác; nhiều giao dịch khác vẫn khớp. Đối tác yêu cầu giữ chính xác 2 chữ số thập phân ở số tiền cuối cùng.

## Bạn cần làm gì

1. Chạy chương trình mẫu, lưu giá trị đầu vào, kết quả và kỳ vọng.
2. Tìm bộ dữ liệu nhỏ nhất tạo sai lệch; viết ít nhất hai giả thuyết trước khi xem hints.
3. Chỉ sửa `starter/DepositRefund.cs`; không sửa `checks/` hoặc thay đổi hợp đồng ở `docs/scenario.md`.
4. Chạy verification và giải thích tại sao các ca âm, không nằm ở điểm giữa, và nhiều đơn vị vẫn đúng.
5. So sánh với reference solution sau khi đã tự thử.

## Yêu cầu môi trường

.NET SDK 8 trở lên, PowerShell 5.1/7 hoặc shell có thể chạy `dotnet`. Không cần database, Docker, Azure hoặc NuGet bên thứ ba.

## Chạy nhanh

Tại thư mục unit:

```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

Sau khi sửa starter:

```powershell
./verify.ps1
```

Trên Linux/macOS có thể dùng `dotnet run --project checks/Checks.csproj -- reproduce` và `-- verify`.

## Cách reproduce vấn đề

Chạy `./reproduce.ps1` để so sánh các trường hợp hoàn cọc một thùng, nhiều thùng và một giao dịch thông thường. Script trả về thành công **khi quan sát được lỗi của starter**, không phải khi đã sửa xong.

## Những gì cần quan sát

- Số tiền trước khi làm tròn, kết quả hệ thống và kết quả theo hợp đồng.
- Sai lệch có xảy ra với mọi số hay chỉ một nhóm dữ liệu?
- Kết quả với số lượng âm có giữ tính đối xứng không?
- Kết quả tính trên tổng có khớp với cách tính từng đơn vị hay không?

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler — chỉ mở sau khi tự điều tra:** [Reference Solution](solution/README.md).

## Expected Results

[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time

35–45 phút. Ghi bằng chứng vào [workspace/my-investigation.md](workspace/my-investigation.md).
