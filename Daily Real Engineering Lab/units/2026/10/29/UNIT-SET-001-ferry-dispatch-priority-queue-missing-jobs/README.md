# UNIT-SET-001 — Hàng đợi điều phối phà thiếu yêu cầu dù không có exception

## Mục tiêu
Điều tra vì sao hệ thống điều phối nhận đủ yêu cầu nhưng màn hình hàng đợi không hiển thị đủ; sửa implementation và chứng minh tính nhất quán.

## Bối cảnh thực tế
Một bến phà nhận năm yêu cầu điều phối xe trong vài giây. Gateway ghi nhận đủ năm yêu cầu hợp lệ, nhưng bảng điều phối chỉ còn hai. Không có exception, hệ thống vẫn trả phản hồi bình thường. Xe có thể bị bỏ khỏi lịch lên phà.

## Bạn cần làm gì
1. Reproduce với fixture cố định; ghi số yêu cầu đầu vào, số được chấp nhận và thứ tự hiển thị.
2. Đưa ra ít nhất ba giả thuyết; kiểm tra code xử lý hàng đợi và bằng chứng.
3. Chỉ chỉnh sửa starter/DispatchQueue.cs; không thay đổi fixture hoặc acceptance checks.
4. Chạy verify, giải thích vì sao không mất yêu cầu và vì sao thứ tự ổn định.
5. So sánh với reference solution và đánh giá rủi ro khi có nhiều producer.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell. Không cần NuGet ngoài, Docker, Azure hoặc database.

## Chạy nhanh
~~~powershell
./run.ps1
./reproduce.ps1
./verify.ps1
~~~
Hoặc: dotnet run --project starter/FerryDispatchLab.csproj -- reproduce (hay verify).

## Cách reproduce vấn đề
Chạy reproduce.ps1 trên starter nguyên bản: exit code 0 nghĩa là đã quan sát đúng triệu chứng. verify.ps1 trả exit code khác 0 cho đến khi bản sửa đáp ứng acceptance checks.

## Những gì cần quan sát
Ghi lại Accepted, Pending, thứ tự JobId và số lần Enqueue trả false. Đối chiếu [incident evidence](evidence/incident.md). Không thay đổi fixture để che triệu chứng.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ mở sau khi tự thử:** [Giải thích](solution/README.md) · [Reference code](solution/DispatchQueue.cs)

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
50 phút · L2 / D4. [Workspace điều tra](workspace/my-investigation.md).
