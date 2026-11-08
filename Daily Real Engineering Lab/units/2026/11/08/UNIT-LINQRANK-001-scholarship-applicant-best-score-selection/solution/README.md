# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms
Hai import cùng dữ liệu nhưng khác thứ tự tạo shortlist khác nhau.

## Evidence
Starter in `Original`, `Reversed` và `FAIL`; revision AN-v1 có thể che khuất AN-v2 dù điểm thấp hơn.

## Root cause
`DistinctBy` giữ phần tử **đầu tiên** theo từng key. Khi deduplicate trước khi xếp hạng, thứ tự arrival trở thành quy tắc chọn revision ngầm định.

## Why the fix works
Sắp xếp từng revision theo Score giảm dần, SubmittedAt mới nhất trước, RevisionId ordinal tăng dần; sau đó `DistinctBy(ApplicantId)` giữ revision tốt nhất mỗi ứng viên. Cuối cùng xếp hạng ứng viên theo Score và ApplicantId để shortlist ổn định. Xem `Ranking.reference.cs` và áp dụng logic vào `starter/Program.cs` theo cách của bạn.

## How to verify
Chạy `./verify.ps1`; phải trả exit code 0 và `PASS`. Đọc thêm các permutation và boundary cases trong starter.

## Alternative fixes
`GroupBy(ApplicantId)` rồi chọn `OrderByDescending(Score).ThenByDescending(SubmittedAt).ThenBy(RevisionId).First()` cho mỗi nhóm cũng đúng, nhưng có chi phí materialization/grouping khác.

## Wrong or misleading fixes
Chỉ thêm `OrderByDescending` **sau** `DistinctBy` không cứu revision đã bị loại. Chỉ sort theo Score mà không tie-break có thể khiến revision được chọn phụ thuộc arrival order khi bằng điểm. `ToList()` chỉ materialize, không sửa semantics.

## Production implications
Thống nhất policy tie-break với nghiệp vụ; log candidate ID, revision ID, version quy tắc và input batch ID để audit; với nguồn dữ liệu lớn cân nhắc `ROW_NUMBER() OVER(PARTITION BY ApplicantId ORDER BY Score DESC, SubmittedAt DESC, RevisionId ASC)` ở database.

## Trade-offs
Sort toàn bộ dữ liệu tốn O(n log n), group có memory overhead; pushdown SQL giảm transfer nhưng cần xác định collation, tie-break và snapshot nhất quán.

## What a Senior engineer should notice
Không được giả định upstream ordering là business invariant. Contract cần xác định revision được chọn trước khi giới hạn top-N; test permutation, ties và batch size chứ không chỉ happy path.
