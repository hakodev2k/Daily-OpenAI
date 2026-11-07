# UNIT-OPT-001 — Partner đổi credential giữa ca: gateway vẫn nhận 401

## Mục tiêu
Tái hiện sự cố khi ứng dụng .NET chạy liên tục qua nhiều lần thay đổi cấu hình, phân biệt tình trạng của configuration provider, service instance và hành vi gọi đối tác. Refactor một thành phần đang hoạt động trong production mà không thay đổi contract của client.

## Bối cảnh thực tế
Một dịch vụ gửi kết quả xét nghiệm đến cổng tiếp nhận của đối tác. Đối tác thay credential theo chính sách định kỳ. Bảng điều khiển cấu hình báo đã áp dụng phiên bản mới; đối tác vẫn hoạt động bình thường, nhưng request từ gateway bắt đầu nhận Unauthorized. Restart process tạm thời khôi phục dịch vụ. Lab dùng **mã phiên bản giả lập v1/v2/v3**, không chứa secret thật, không gọi Internet hay dịch vụ Azure.

## Bạn cần làm gì
1. Chạy starter và ghi nhận ba lần gửi trước/sau hai đợt rotation.
2. Thu thập evidence từ configuration, gateway outcome và control call trực tiếp tới đối tác giả lập.
3. Đề xuất tối thiểu ba hypothesis; ghi cách bác bỏ từng hypothesis.
4. Chỉ sửa `starter/PartnerGateway.cs`; giữ nguyên harness và `PartnerEmulator`.
5. Chạy verify để chứng minh request mới dùng đúng phiên bản sau cả hai lần rotation. Giải thích giới hạn khi scale-out.

## Yêu cầu môi trường
.NET SDK 8 (8.0.100+), PowerShell 5.1 hoặc 7; không cần Docker, NuGet ngoài .NET shared framework, hay tài khoản cloud.

## Chạy nhanh
Từ thư mục unit:

~~~powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
~~~

## Cách reproduce vấn đề
~~~powershell
./scripts/reproduce.ps1
~~~

`INCIDENT_REPRODUCED` và exit code 0 nghĩa là **starter đã tái hiện lỗi**, không có nghĩa đã sửa. `verify.ps1` phải thất bại trên starter gốc và thành công khi bạn sửa `starter/PartnerGateway.cs`.

## Những gì cần quan sát
- Lần gửi đầu được chấp nhận; hai lần sau nhận trạng thái gì?
- Provider cho biết version nào sau mỗi thay đổi?
- Control call trực tiếp đến đối tác trả kết quả gì, và điều đó loại trừ hypothesis nào?
- Vì sao restart có thể che khuất nguyên nhân mà không giải quyết vòng đời cập nhật?
- Khi một request đang chạy, nên chọn thời điểm nào để cố định credential version của request đó?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trong starter.
4. Verify cả regression và hai lần rotation.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ xem sau khi tự sửa:** [Giải thích và code tham chiếu](solution/README.md).

## Expected Results
[Trước sửa](expected-results/before.md) · [Sau sửa](expected-results/after.md)

## Estimated Time
60–75 phút (L4 Refactoring Workshop, D4).
