# UNIT-RETENTION-001 — Thiết kế vòng đời bằng chứng số: retention, legal hold và quyền xóa

## Mục tiêu
Thiết kế và bảo vệ một quyết định kỹ thuật về vòng đời bằng chứng số dựa trên bằng chứng, ràng buộc kinh doanh và failure paths; phân biệt trạng thái nghiệp vụ với cơ chế lưu trữ.

## Bối cảnh thực tế
Cổng hỗ trợ pháp lý cộng đồng lưu PDF, ảnh và biên bản của từng hồ sơ. Sau đợt triển khai chính sách lưu trữ mới, bộ phận vận hành phát hiện hồ sơ cần bảo toàn tài liệu nhưng file không còn; đồng thời một số yêu cầu xóa đã quá hạn vẫn còn object. Không có quyền sử dụng Azure thực tế trong lab.

## Bạn cần làm gì
1. Đọc [case timeline](evidence/case-timeline.csv), [yêu cầu và ràng buộc](docs/scenario.md).
2. Điền [decision worksheet](workspace/decision.md): lập state machine, invariants, đánh giá tối thiểu ba phương án, kế hoạch migration và rollback.
3. Dùng [acceptance cases](evidence/acceptance-cases.md) để tự kiểm tra thiết kế; ghi bằng chứng cho từng trường hợp.
4. Chỉ sau khi hoàn thành, đối chiếu [reference solution](solution/README.md).

## Yêu cầu môi trường
Trình đọc Markdown/CSV và Git; không cần cloud subscription, database hoặc Docker.

## Chạy nhanh
```powershell
git pull
cd "Daily Real Engineering Lab/units/2026/11/08/UNIT-RETENTION-001-legal-aid-evidence-retention-policy"
Get-Content .\docs\scenario.md
Import-Csv .\evidence\case-timeline.csv | Format-Table
```

## Cách reproduce vấn đề
Đối chiếu mỗi hàng timeline với quy tắc hiện tại trong scenario. Xác định những hàng có hành vi không đáp ứng business invariant. Không suy đoán rằng mọi yêu cầu xóa đều phải thực hiện ngay.

## Những gì cần quan sát
Ghi rõ thời điểm, loại yêu cầu, trạng thái hồ sơ, trạng thái object và ai đã ra quyết định. Phân biệt thiếu evidence với policy violation đã được chứng minh.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix bằng một thiết kế và migration plan.
4. Verify với acceptance cases.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Một phương án có thể bảo vệ](solution/README.md).

## Expected Results
Trước: policy toàn cục có thể đưa ra quyết định sai trong các tình huống khác nhau. Sau: mỗi quyết định có trạng thái, căn cứ, dấu vết kiểm toán và cơ chế reconcile; không có xóa khi hold còn hiệu lực.

## Estimated Time
75 phút.
