# UNIT-EF-018 — Kho đạo cụ nhà hát: số lượng khả dụng lệch sau hai nghiệp vụ

## Mục tiêu
Điều tra một sai lệch tồn kho trong ứng dụng EF Core, bảo toàn quy tắc số lượng khả dụng và refactor mà không phá vỡ các luồng đang chạy.

## Bối cảnh thực tế
Một nhà hát có hệ thống cấp phát đạo cụ cho sân khấu và quầy cho thuê. Hai thao tác thực hiện liên tiếp trong cùng một request: đội sân khấu giữ trước một số đạo cụ; nhân viên quầy sau đó hoàn tất giao dịch. Báo cáo cuối ngày đôi khi cho thấy số lượng khả dụng cao hơn thực tế. Trường hợp khác, quầy vẫn chấp nhận giao dịch khi không còn đủ hàng.

## Bạn cần làm gì
- Chạy mẫu dữ liệu nhỏ để ghi nhận số lượng trước/sau từng nghiệp vụ.
- Đưa ra ít nhất hai giả thuyết và xác định bằng chứng phân biệt chúng.
- Sửa mã trong `starter/Program.cs`, giữ nguyên các quy tắc nghiệp vụ.
- Chạy lại ba tình huống, bao gồm một giao dịch phải bị từ chối.
- Giải thích giới hạn của bản sửa khi có hai request đồng thời.

## Yêu cầu môi trường
PowerShell 7 hoặc Windows PowerShell; .NET SDK 8+; mạng để restore NuGet lần đầu. Không cần database server, Azure hoặc Docker; dùng SQLite in-memory.

## Chạy nhanh
~~~powershell
cd 'Daily Real Engineering Lab/units/2026/10/30/UNIT-EF-018-theatre-prop-reservation-tracker-drift'
./setup.ps1
./run.ps1
~~~

## Cách reproduce vấn đề
~~~powershell
./reproduce.ps1
~~~
Exit code 0 ở bản starter nguyên gốc có nghĩa là **triệu chứng được tái hiện**, không phải ứng dụng đã đúng. Sau khi sửa, lệnh này có thể không còn nhận diện lỗi cũ.

## Những gì cần quan sát
Với 10 đạo cụ ban đầu, đội sân khấu giữ 2 và quầy cho thuê 1: số lượng hợp lệ cuối cùng là 7. Đối chiếu với số lượng persisted mà chương trình in ra. Với 3 đạo cụ, giữ 2 rồi yêu cầu thuê 2: quầy phải từ chối và số lượng còn 1. Ghi lại cả kết quả chấp nhận/từ chối.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ xem [reference solution](solution/README.md) sau khi tự điều tra và verify.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
60–75 phút. Tham khảo [scenario](docs/scenario.md) và ghi chép tại [workspace](workspace/my-investigation.md).
