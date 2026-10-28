# Reference Solution — SPOILER

**Chỉ đọc sau khi reproduce, lập hypothesis và tự thử sửa.** File tham khảo: [query.js](query.js). `verify.ps1` cố ý đọc `starter/query.js`, không đọc file solution.

## 1. Symptoms
Routing chọn 5 turbine trong khi chỉ có 3 turbine chứa một inspection phanh thất bại. `WT-041` và `WT-046` là false positives.

## 2. Evidence
Cả hai turbine false-positive đều có một inspection phanh `PASS` và một inspection loại khác `FAILED`. Không có timeout hoặc lỗi MongoDB trong fixture.

## 3. Root cause
MongoDB cho phép các điều kiện dotted path `"inspections.type":"brake"` và `"inspections.status":"FAILED"` khớp trên **hai phần tử khác nhau** của cùng một array. Query cũ chỉ yêu cầu tồn tại ít nhất một phần tử có type brake và ít nhất một phần tử có status FAILED, chứ không ràng buộc cùng inspection.

## 4. Why the fix works
`$elemMatch: {type:"brake", status:"FAILED"}` yêu cầu một embedded document duy nhất trong `inspections` thỏa cả hai predicate. `WT-041` và `WT-046` bị loại, `WT-042`, `WT-044`, `WT-045` được giữ lại.

## 5. Verification
Sửa duy nhất `starter/query.js`, chạy `./verify.ps1`. Script reset dữ liệu, kiểm tra 3 IDs chuẩn rồi chèn hai regression cases để kiểm tra khả năng phân biệt cùng phần tử và khác phần tử. Không thay fixture hoặc test để tạo pass giả.

## 6. Alternatives
Có thể dùng aggregation pipeline với `$filter` hoặc `$anyElementTrue`, nhưng phức tạp hơn khi chỉ cần kiểm tra tồn tại một phần tử. Có thể lưu mỗi inspection thành collection riêng và truy vấn theo turbine, nhưng tăng chi phí schema, indexing và read path; chỉ đáng cân nhắc nếu nhu cầu truy vấn/ghi riêng từng inspection đủ lớn.

## 7. Wrong / tempting fixes
- Chỉ tăng độ ưu tiên worker hoặc retry: không thay đổi tập kết quả logic.
- Lọc `status:FAILED` trong application sau khi query: có thể đúng nếu kiểm tra từng inspection, nhưng tăng dữ liệu đọc và dễ bỏ sót tại các code path khác.
- Dùng `$and` bọc hai dotted predicates: vẫn không buộc chúng thuộc cùng array element.
- Bỏ các turbine false-positive bằng hard-coded IDs: che triệu chứng, phá tính tổng quát.

## 8. Production implications
False-positive routing gây lãng phí điều phối và làm méo KPI maintenance. Thêm metrics cho candidate/confirmed divergence, sampling audit theo loại inspection và contract tests trên array có nhiều trạng thái trái ngược.

## 9. Trade-offs
`$elemMatch` giữ query đơn giản, server-side, ít dữ liệu trả về. Khi dataset lớn, đánh giá compound multikey index phù hợp, kiểm tra `explain()` trên workload thực tế và không mặc định index là nguyên nhân logic.

## 10. Senior engineer should notice
Cần phân biệt predicate trên document với predicate trên **cùng một phần tử** trong array; xây regression fixtures cố tình có điều kiện chia qua nhiều phần tử, tránh chỉ test các document có một inspection. Điều tra tính đúng trước khi tối ưu hiệu năng hoặc scale.
