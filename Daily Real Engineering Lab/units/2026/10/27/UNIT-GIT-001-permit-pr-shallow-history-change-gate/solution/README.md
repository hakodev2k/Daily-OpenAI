# Reference Solution — spoiler

Chỉ đọc sau khi reproduce và tự sửa starter/detect-changes.ps1. File solution/detect-changes.ps1 là một reference implementation cho fixture hiện tại, không phải template CI áp dụng mù quáng cho mọi provider.

## 1. Symptoms

CI có source của HEAD nhưng change gate không trả được tập file thay đổi. Targeted tests không được chọn. Reproduce xác nhận exit 12 trên shallow checkout.

## 2. Evidence

- Git xác nhận checkout là shallow.
- Commit HEAD có sẵn, nhưng parent hoặc comparison base chưa có object tương ứng trong local object database.
- Base SHA được ghi rõ ở .lab-runtime/base.sha và có trên remote main.
- Fixture PR chỉ thay đổi hai path: docs/permit-rules.md và src/PermitEngine/Rules.cs.

## 3. Root cause

Gate dùng HEAD^ như một giả định về lịch sử local và điểm so sánh PR. Checkout shallow depth 1 không có đủ parent object, nên Git không resolve được HEAD^. Ngay cả khi parent có sẵn, HEAD^ cũng không luôn là PR base (PR nhiều commit, merge commits, rebases).

## 4. Why the fix works

Reference script kiểm tra định dạng SHA, xác minh working tree, fetch commit base từ nhánh main của fixture với độ sâu nhỏ, xác minh commit object rồi diff hai tree BaseSha và HEAD. Mọi trường hợp thiếu base hoặc Git lỗi đều trả exit khác 0. Fixture cố ý đặt main tại BaseSha, nên fetch --depth=1 origin main đủ để truy cập base.

## 5. How to verify

Sao chép nội dung reference script vào starter/detect-changes.ps1 rồi chạy ./verify.ps1. Verify kiểm tra chính xác hai path và kiểm tra base SHA không tồn tại phải thất bại. Reproduce vẫn dùng baseline bất biến.

## 6. Alternative fixes

- CI nhỏ: fetch full history rồi diff theo explicit base SHA. Dễ hiểu nhưng có thể tốn bandwidth/time cho monorepo.
- CI lớn: fetch có chọn lọc hoặc deepen theo giới hạn, rồi xác minh base commit trước khi diff.
- Với PR cần merge-base: xác định base đúng theo event/provider; fetch đủ ancestry và dùng git merge-base, thay vì lấy HEAD^.
- Với GitHub Actions: cấu hình checkout fetch-depth theo nhu cầu, nhưng vẫn xác định đúng PR base và head; fetch-depth 0 không tự sửa logic so sánh sai.

## 7. Wrong / tempting fixes

- Bỏ qua lỗi và trả empty change set: false negative có thể bỏ qua security/quality tests.
- Dùng HEAD^ khi fetch-depth tăng lên 2: chỉ sửa thiếu parent trong một số trường hợp; không đảm bảo PR base đúng.
- Luôn chạy tất cả tests: có thể là fallback fail-safe hợp lý, nhưng không sửa gate và có chi phí CI.
- Tắt kiểm tra trên runner: che giấu sai lệch giữa local và CI.

## 8. Production implications

Ghi log base/head SHA, shallow state, fetch strategy, diff exit code và số path; không log token. Đặt timeout và giới hạn fetch, dùng fallback full test suite khi không thể tính change set nếu yêu cầu compliance. Phân biệt pull_request, push, merge queue và rebase event.

## 9. Trade-offs

Fetch full history đơn giản nhưng chi phí cao; fetch tối thiểu tiết kiệm hơn nhưng cần hợp đồng commit rõ ràng và xử lý history rewrite. BaseSha được truyền vào phải là commit đáng tin từ CI event, không được lấy tùy tiện từ input không kiểm soát.

## 10. Senior engineering insight

Một change detector là correctness boundary của test selection. Sai kết quả rỗng nguy hiểm hơn lỗi hiển thị rõ. Thiết kế phải phân biệt missing Git objects, wrong comparison base và empty diff hợp lệ. Thử cả shallow clone, multi-commit PR, force-push/rebase và target branch moving trước khi áp dụng cho production.
