# UNIT-CACHEKEY-001 — Cache dữ liệu dự báo trả nhầm trạm đo

## Mục tiêu
Điều tra sai lệch dữ liệu khi bật cache, xác lập invariant cách ly giữa các trạm và bổ sung regression test trên chính code nghiệp vụ.

## Bối cảnh thực tế
Một API cung cấp dự báo theo trạm và ngày. Khi nhiều trạm được truy vấn trong cùng ngày, bảng điều khiển đôi khi hiển thị nhiệt độ của trạm khác dù upstream vẫn chính xác.

## Bạn cần làm gì
1. Reproduce với hai trạm cùng ngày.
2. Thu thập evidence: request, stationId, cache hit/miss, kết quả.
3. Đưa ra ít nhất ba giả thuyết, sửa `starter/Program.cs`.
4. Verify trường hợp cùng ngày khác trạm, cùng trạm khác ngày và cache hit hợp lệ.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7+. Không cần network hay NuGet bên ngoài SDK.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` từ thư mục unit trước khi sửa. Sau đó chạy `./verify.ps1` để kiểm thử code trong `starter/`.

## Những gì cần quan sát
Đối chiếu danh tính trạm ở request và response, số lần tải upstream, và tính ổn định khi đảo thứ tự request. Không suy luận nguyên nhân chỉ từ một response sai.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Solution](solution/README.md) — chỉ đọc sau khi tự sửa.

## Expected Results
Trước: một chuỗi request trả dữ liệu của sai trạm. Sau: các invariant isolation, freshness và reuse đều PASS.

## Estimated Time
45 phút.
