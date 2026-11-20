# UNIT-HASHKEY-001 — Museum object lookup drift

## Mục tiêu
Điều tra một lỗi tra cứu hiện vật sau khi cập nhật vị trí; phân biệt dữ liệu hiển thị với hợp đồng tra cứu và thiết kế phép kiểm chứng hồi quy.

## Bối cảnh thực tế
Bảo tàng quản lý các hiện vật mượn theo mã hiện vật và phòng trưng bày. Sau thao tác chuyển phòng, màn hình audit vẫn thấy hiện vật nhưng bộ phận vận chuyển không tra cứu được theo vị trí mới. Không có lỗi mạng hoặc database trong bản mô phỏng.

## Bạn cần làm gì
Tái hiện sự cố, ghi ít nhất hai giả thuyết, xác định bất biến bị vi phạm, sửa code trong `starter/Program.cs`, chạy verification và đánh giá rủi ro nếu có nhiều luồng cập nhật.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7 hoặc Windows PowerShell; không cần NuGet ngoài SDK.

## Chạy nhanh
Từ thư mục unit:
```powershell
dotnet run --project starter/ExhibitLookup.csproj
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` trên bản starter nguyên gốc. Script yêu cầu quan sát đúng triệu chứng và trả về thành công khi lỗi được tái hiện. Nếu bạn đã sửa starter, dùng Git để phục hồi bản gốc trước khi reproduce.

## Những gì cần quan sát
So sánh số lượng bản ghi, key được liệt kê và kết quả lookup bằng hai cách. Ghi lại liệu thao tác di chuyển có làm mất dữ liệu thật hay chỉ ảnh hưởng đường truy xuất.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ xem sau khi tự thử sửa: [solution/README.md](solution/README.md).

## Expected Results
Trước: `count=1`, audit hiển thị vị trí mới, nhưng `RESULT found=NO`. Sau: `count=1`, lookup bằng key mới trả về `RESULT found=YES`. Không phụ thuộc timing.

## Estimated Time
35–45 phút.
