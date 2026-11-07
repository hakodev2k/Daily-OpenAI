# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Một edit semantic một dòng xuất hiện như thay đổi toàn bộ file, làm PR khó review.

## 2. Evidence
`git diff --numstat` cho thấy gần mọi dòng bị xóa/thêm. Nội dung hiển thị gần như giống nhau; khác biệt hệ thống nằm ở line-ending bytes. `git check-attr` cho thấy repository không sở hữu policy normalization.

## 3. Root cause
Repository không có line-ending contract rõ ràng. Worktree từ các môi trường khác nhau có thể chuyển LF và CRLF, khiến Git thấy mọi dòng thay đổi cùng lúc với edit thật.

## 4. Why the fix works
Thêm `* text=auto eol=lf` vào `starter/.gitattributes` làm normalization trở thành policy version-controlled. Sau một lần renormalize có chủ đích, các edit sau không phụ thuộc line-ending preference của từng developer.

## 5. How to verify
Thêm dòng policy trên, chạy `./verify.ps1`. Verification tạo repository mới từ learner-editable starter, mô phỏng CRLF cùng một semantic edit, renormalize rồi yêu cầu diff còn đúng 1 add/1 delete.

## 6. Alternative fixes
Có thể dùng policy theo extension, ví dụ chỉ normalize source/config text và giữ scripts Windows cần CRLF. Với repository có file đặc thù, policy chi tiết thường tốt hơn wildcard tuyệt đối.

## 7. Wrong or misleading fixes
Đổi `core.autocrlf` chỉ trên máy của một developer không tạo contract chung cho team. Bỏ qua diff lớn và merge vẫn để review noise tái diễn. Dùng whitespace-ignore trong review có thể che triệu chứng nhưng không sửa representation trong repository.

## 8. Production implications
Diff noise làm giảm chất lượng review, tăng merge conflicts và có thể kích hoạt build/deploy không cần thiết. Một lần renormalization nên được tách thành PR riêng để không trộn với business changes.

## 9. Trade-offs
LF là lựa chọn cross-platform phổ biến, nhưng một số Windows-native artifacts có thể cần CRLF. `.gitattributes` nên mô tả repository thực tế thay vì áp dụng policy máy cá nhân một cách mù quáng.

## 10. What a Senior engineer should notice
Vấn đề không phải editor nào “đúng”; đó là thiếu repository-level contract. Senior engineer nên làm normalization deterministic, plan migration riêng, và bảo vệ review signal-to-noise cho cả team.
