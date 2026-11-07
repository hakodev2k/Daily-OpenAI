# UNIT-FLAG-001 — Beta rollout đổi nhóm khi chuyển replica

## Mục tiêu
Điều tra tính nhất quán của progressive feature rollout khi cùng một tài khoản được phục vụ bởi các replica khác nhau; thực hiện refactoring có regression test.

## Bối cảnh thực tế
Cổng theo dõi nước sinh hoạt thử nghiệm biểu đồ mới cho 20% tài khoản. Sau khi chuyển traffic giữa hai replica, người dùng phản ánh tính năng bật/tắt thất thường dù tổng tỷ lệ enable trên dashboard vẫn gần 20%.

## Bạn cần làm gì
Chạy fixture, thu thập bucket và kết quả cho cùng danh tính, đưa ra ít nhất hai hypothesis, sửa starter/FlagEvaluator.cs mà không chỉnh sửa test harness. Đánh giá rủi ro migration khi thay đổi thuật toán trên production.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell; không cần cloud, Docker hoặc external NuGet packages. Xem global.json.

## Chạy nhanh
~~~powershell
./run.ps1
./reproduce.ps1
./verify.ps1
~~~
Hoặc: dotnet run --project ./starter/FlagRollout.csproj -- reproduce

## Cách reproduce vấn đề
Chạy reproduce.ps1 trên starter nguyên bản. Fixture so sánh quyết định cohort cho cùng identity qua hai replica mô phỏng. reproduce trả exit code 0 khi tìm được triệu chứng; verify trả nonzero trước khi sửa.

## Những gì cần quan sát
Bucket của cùng identity có giống nhau giữa replica A/B? Aggregate enable rate có phản ánh user-level stability? Kiểm tra ngưỡng 0%, 100%, rollout 20%→40% và tenant separation.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải thích](solution/README.md) · [Reference code](solution/FlagEvaluator.cs)

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
60–75 phút, L4 / D4. [Investigation workspace](workspace/my-investigation.md).
