# UNIT-LINQSNAP-001 — Snapshot báo cáo suất ăn học đường
## Mục tiêu
Điều tra sự sai lệch giữa tổng số và chi tiết của một bản xuất, xây dựng bằng chứng và regression test.
## Bối cảnh thực tế
Một dịch vụ tổng hợp suất ăn theo trường. Khi dữ liệu phát sinh trong lúc tạo báo cáo, tổng số và danh sách chi tiết không khớp, gây khó khăn đối soát.
## Bạn cần làm gì
Chạy starter, ghi evidence và ít nhất hai hypotheses, sửa starter rồi verify.
## Yêu cầu môi trường
.NET SDK 8.0, PowerShell 7 (hoặc chạy dotnet trực tiếp), không cần NuGet ngoài SDK.
## Chạy nhanh
`dotnet run --project starter/SchoolMealAudit.csproj`
## Cách reproduce vấn đề
`pwsh ./reproduce.ps1` (hoặc chạy dotnet trực tiếp và kiểm tra exit code).
## Những gì cần quan sát
So sánh reportedCount, detailCount, sourceCount và thời điểm nguồn thay đổi. Không coi việc nguồn có bản ghi mới là lỗi nghiệp vụ.
## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.
## Hints
[Hint 1](hints/hint-1.md), [Hint 2](hints/hint-2.md), [Hint 3](hints/hint-3.md)
## Reference Solution
**Spoiler:** [Reference](solution/README.md).
## Expected Results
Starter: số lượng báo cáo và chi tiết khác nhau, exit 1. Fixed: cả hai bằng 2, nguồn có 3, exit 0.
## Estimated Time
45–60 phút.
