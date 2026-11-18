# UNIT-JSSAFEINT-001 — Sai lệch mã công tơ điện mặt trời khi đối soát JSON

## Mục tiêu
Điều tra một sai lệch định danh xuyên ranh giới .NET và JavaScript; xây dựng hợp đồng dữ liệu có kiểm chứng, không chỉ sửa ví dụ.

## Bối cảnh thực tế
Nền tảng thanh toán điện mặt trời nhận mã công tơ 64-bit từ .NET API. Dashboard đối soát hiển thị đôi lúc hai mã khác nhau thành cùng một mã, khiến thao tác chọn bản ghi sai.

## Bạn cần làm gì
Chạy hệ thống mô phỏng, lưu bằng chứng về dữ liệu trước và sau khi đi qua client, viết hypothesis, rồi sửa mã trong `starter/Program.cs`. Giữ nguyên mã công tơ, số lượng bản ghi và khả năng round-trip.

## Yêu cầu môi trường
.NET SDK 8; PowerShell 7 hoặc Windows PowerShell 5.1. Không cần Azure, database hay npm.

## Chạy nhanh
```powershell
dotnet run --project starter/MeterReplay.csproj
./reproduce.ps1
```

## Cách reproduce vấn đề
Tại thư mục unit, chạy `./reproduce.ps1`. Script yêu cầu chương trình trả về dấu hiệu sai lệch để xác nhận đã reproduce. Không sửa starter trước khi thu thập evidence.

## Những gì cần quan sát
Ghi lại hai mã đầu vào, JSON trên wire, mã client nhìn thấy và số lượng distinct IDs. Đối chiếu với `evidence/incident.md`.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong starter.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Solution](solution/README.md) — chỉ xem sau khi thử fix.

## Expected Results
Trước: hai mã hợp lệ trở thành một mã phía client. Sau: mã vẫn phân biệt và round-trip chính xác; các trường số đo vẫn là số.

## Estimated Time
45–60 phút.

