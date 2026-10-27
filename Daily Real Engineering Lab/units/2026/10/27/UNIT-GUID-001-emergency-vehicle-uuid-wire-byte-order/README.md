# UNIT-GUID-001 — Emergency Dispatch: UUID không khớp giữa .NET và đối tác

## Mục tiêu
Điều tra sự cố định danh xe cứu hộ giữa backend .NET và một hệ thống đăng ký thiết bị khác ngôn ngữ. Phân biệt local round-trip với cross-language wire contract.

## Bối cảnh thực tế
Một gateway gửi UUID của xe cứu hộ qua binary frame 16 byte. Unit test encode rồi decode trong cùng .NET service đều xanh, nhưng partner registry trả unknown vehicle. Đội vận hành cung cấp hai golden vectors độc lập.

## Bạn cần làm gì
1. Reproduce; ghi lại output và raw byte sequences.
2. Đề xuất ít nhất ba hypothesis và kiểm tra evidence.
3. Sửa duy nhất starter/VehicleEventCodec.cs, không sửa checks/Program.cs.
4. Verify cả encode và decode bằng golden vectors; giữ negative test cho frame ngắn.
5. So sánh reference solution sau khi tự sửa.

## Yêu cầu môi trường
.NET 8 SDK, PowerShell 7+ hoặc Windows PowerShell 5.1. Không cần Docker, database, network hay NuGet ngoài framework.

## Chạy nhanh
~~~powershell
dotnet --version
dotnet build ./checks/ContractChecks.csproj -c Release
./reproduce.ps1
~~~

## Cách reproduce vấn đề
Chạy ./reproduce.ps1 từ thư mục unit. Exit code 0 có nghĩa là **triệu chứng starter được chứng minh**, không phải code đã sửa. Sau khi sửa starter, chạy ./verify.ps1.

## Những gì cần quan sát
- Local round-trip trả về đúng vehicle ID.
- Frame gửi ra không khớp golden vector.
- Frame hợp lệ từ đối tác có thể được hiểu thành vehicle ID khác.
- Tất cả fixture đều deterministic, không có external dependency.

Ghi evidence vào [workspace](workspace/my-investigation.md), xem [partner vectors](evidence/partner-vectors.md) và [investigation guide](docs/investigation-guide.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Reference solution và root cause](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time
60–75 phút. L3 Production Incident / D4 Production Troubleshooting.
