# Reference Solution — chỉ xem sau khi tự reproduce và thử sửa

## 1. Symptoms

CI liệt kê file không thuộc feature, khiến build/test scope tăng và reviewer nhầm phần thay đổi của Pull Request.

## 2. Evidence

Case-a: feature chỉ thêm `src/Enrollment/EnrollmentService.cs`, main chỉ sửa `src/Shared/Clock.cs`, script cũ trả cả hai. Case-b: feature sửa Approval, main thêm ReleaseNote. Case-c: feature không có commit mới nhưng script vẫn trả file của main.

## 3. Root cause

`git diff main feature` so sánh trực tiếp **hai snapshot ở branch tips**. Nó không có nghĩa “những thay đổi feature đưa vào từ lúc tách nhánh”. Khi main tiến thêm, kết quả hai đầu nhánh chứa cả thay đổi riêng của main.

## 4. Why the fix works

Dùng tổ tiên chung (`git merge-base main feature`) làm điểm bắt đầu rồi so với feature head. Dạng ngắn tương đương trong fixture: `git diff --name-only main...feature`. File chỉ thuộc main không xuất hiện; file được thêm/sửa/xóa trong feature vẫn có mặt. Xem [reference script](change-detection.ps1). Không sửa verify để gọi solution: verify phải kiểm tra starter.

## 5. How to verify

Chỉ sửa `starter/change-detection.ps1`, chạy `.\verify.ps1`. Case-a chỉ EnrollmentService; case-b chỉ Approval; case-c rỗng. Đối chiếu bằng `git -C workspace/fixtures/case-a log --graph --oneline --all`.

## 6. Alternative fixes

- `git diff --name-only main...feature`: ngắn, hợp PR change detection.
- `git merge-base` rồi `git diff <base-sha> feature`: minh bạch, có thể log lỗi thiếu common ancestor.
- Pull Request changed-files API: đúng semantics của nền tảng khi CI có quyền, nhưng thêm network, permission và pagination.

## 7. Wrong / tempting fixes

- Lọc path theo tên project: có thể pass một fixture nhưng bỏ sót thay đổi hợp lệ.
- Dùng `git diff main feature` rồi bỏ file có trên main: không giải quyết trường hợp hai nhánh cùng sửa file.
- `git diff HEAD~1 HEAD`: bỏ sót các commit trước đó của feature.
- Chỉ thêm CI cache/runner: che chi phí chứ không sửa change detection.

## 8. Production implications

Impact list sai có thể build nhầm project, skip test quan trọng hoặc báo false-positive. Shallow clone cần đủ lịch sử để tìm merge base; nếu không, fail rõ ràng hoặc fetch thêm lịch sử có giới hạn, không trả rỗng rồi báo xanh giả.

## 9. Trade-offs

Merge-base-to-head phản ánh thay đổi của feature so với điểm phân nhánh, không phải toàn bộ diff hai snapshot đang deploy. Rebase/merge từ main thay đổi tổ tiên chung hợp lệ. Lịch sử phức tạp có nhiều merge base cần chính sách rõ và regression riêng.

## 10. What a Senior engineer should notice

Phân biệt **PR impact**, **release delta** và **deployed-state diff**; chúng có thể đòi ba phép so sánh khác nhau. Chọn phép tính từ business contract. Fixture phải có main tiến riêng và feature không đổi.

## Giới hạn validation

Đã review tĩnh, chưa thực thi Git/PowerShell trong môi trường phát hành. Chạy script trên máy người học trước khi áp dụng vào CI thật.
