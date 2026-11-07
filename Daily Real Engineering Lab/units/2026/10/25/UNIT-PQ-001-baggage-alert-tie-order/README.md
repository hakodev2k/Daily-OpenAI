# UNIT-PQ-001 — Baggage Alert Dispatch: Equal-Severity Ordering

## Mục tiêu
Điều tra một sự cố về thứ tự xử lý alert trong dịch vụ phân loại hành lý: alert khẩn phải được ưu tiên, nhưng các alert có cùng mức độ khẩn phải được xử lý theo thứ tự tiếp nhận. Dùng bằng chứng thực thi để đánh giá contract, sửa implementation và bổ sung regression checks.

## Bối cảnh thực tế
Tại một sân bay, dịch vụ nhận các alert từ hệ thống kiểm tra hành lý. Quy tắc nghiệp vụ: severity 0 phải đi trước severity 1; nếu severity bằng nhau, alert đến trước phải được dispatch trước. Đợt triển khai mới vẫn ưu tiên alert khẩn, nhưng một số alert cùng severity bị đảo thứ tự. Đây là lỗi ảnh hưởng audit trail và thứ tự xử lý, không chỉ là vấn đề hiển thị.

## Bạn cần làm gì
1. Chạy bản starter, tái hiện sai khác giữa expected và actual.
2. Ghi ít nhất hai hypothesis về nguyên nhân; chỉ ra evidence để bác bỏ từng hypothesis.
3. Chỉ chỉnh sửa logic điều phối trong starter/Program.cs. Giữ nguyên kiểm thử và contract.
4. Chạy verify.ps1; xác nhận cả thứ tự ưu tiên lẫn thứ tự tiếp nhận trong mỗi nhóm severity.

## Yêu cầu môi trường
- .NET SDK 8.x, PowerShell 7+ hoặc Windows PowerShell.
- Không cần Docker, Azure, database hoặc NuGet package ngoài SDK.

## Chạy nhanh
Từ thư mục unit, chạy:

~~~powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
./scripts/verify.ps1
~~~

Không dùng PowerShell:

~~~bash
dotnet run --project starter/BaggageDispatch.csproj -- demo
dotnet run --project starter/BaggageDispatch.csproj -- reproduce
dotnet run --project starter/BaggageDispatch.csproj -- verify
~~~

## Cách reproduce vấn đề
Chạy reproduce.ps1 **trước khi sửa**. Exit code 0 nghĩa là đã tái hiện đúng symptom ban đầu; exit code 2 nghĩa là không còn quan sát được symptom. verify.ps1 có nhiệm vụ khác: phải exit code 1 trước fix và exit code 0 sau fix.

## Những gì cần quan sát
- Dữ liệu đầu vào và severity không thay đổi, nhưng actual dispatch order không khớp expected.
- Kết quả của các trường hợp chỉ có một severity và trường hợp có nhiều severity.
- Sự khác biệt giữa việc thỏa điều kiện ưu tiên và việc giữ đúng thứ tự trong nhóm cùng ưu tiên.
- Không kết luận root cause chỉ dựa vào tên collection hoặc thứ tự của một lần chạy.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: [Chỉ mở sau khi tự điều tra và thử sửa](solution/README.md).

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
40 phút. Ghi kết quả vào [workspace/my-investigation.md](workspace/my-investigation.md).
