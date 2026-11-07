# Reference Solution — SPOILER

Chỉ đọc sau khi đã reproduce, ghi giả thuyết và thử sửa trong starter.

## 1. Symptoms
Hai pod Ready, dùng cùng image reference trong manifest, nhưng kết quả lập tuyến khác nhau; node-east chạy revision A, node-west chạy revision B.

## 2. Evidence
node-east không pull image mới, node-west resolve từ registry; CPU và Redis ổn định; cả hai HTTP 200. Cần so sánh imageID/digest thực chạy, không chỉ trường image của pod spec.

## 3. Root cause
`DeploymentPlanner.Plan` bỏ qua `approvedDigest`, dùng tag `:stable` có thể thay đổi. Với `IfNotPresent`, node-east có cache tag cũ nên giữ A; node-west chưa có image nên lấy B. Tag không xác định duy nhất artifact. Rollback bằng tag cũng không bảo đảm quay về revision đã phê duyệt.

## 4. Why the fix works
File `DeploymentPlanner.cs` ở thư mục này là mã thay thế cho file tương ứng trong `starter/`. Nó kiểm tra SHA-256 digest và phát hành `repository@sha256:...` bất biến. Mọi node resolve cùng artifact; thay đổi digest làm thay đổi pod template; rollback dùng lại digest A.

## 5. How to verify
```powershell
Copy-Item './solution/DeploymentPlanner.cs' './starter/DeploymentPlanner.cs' -Force
./verify.ps1
```
`verify.ps1` chỉ build/chạy mã ở `starter/`, không tự động dùng reference solution. Trước khi copy, nên tự sửa và chạy verify để có bằng chứng độc lập.

## 6. Alternative fixes
`imagePullPolicy: Always` với mutable tag có thể giảm sự khác nhau trong một thời điểm, nhưng tag có thể bị di chuyển giữa các lần pull và rollback không còn xác định. Pin digest ở manifest là hợp đồng phát hành chặt hơn. Registry tag vẫn hữu ích cho discovery, không nên là nguồn sự thật của artifact được phê duyệt.

## 7. Wrong or misleading fixes
- Scale thêm node: tăng phạm vi không nhất quán.
- Xóa image cache trên mọi node: chữa triệu chứng tạm thời, không khóa identity của release.
- Restart pod: có thể thay đổi kết quả theo thời điểm registry được hỏi.
- Chỉ xem Ready/HTTP 200: chứng minh service hoạt động, không chứng minh revision đúng.

## 8. Production implications
CI phải ghi digest của artifact sau build/push; CD dùng digest đó, lưu change record và rollback target. Trong Kubernetes, so sánh `.spec.containers[].image` với `.status.containerStatuses[].imageID`; cần quan sát thực tế container runtime và chính sách registry. Quản lý retention để digest cũ còn pull được khi rollback.

## 9. Trade-offs
Digest pinning giảm linh hoạt của tag mutable, đòi hỏi pipeline quản lý provenance và retention. Đổi lại: reproducibility, audit, rollback và incident triage đáng tin cậy hơn. `IfNotPresent` với digest bất biến vẫn hợp lệ; `Always` có thể hữu ích tùy yêu cầu kiểm tra registry, nhưng không thay thế artifact identity.

## 10. Senior engineer should notice
Phân biệt desired state với observed state; điều tra từng node; kiểm tra binary provenance trước khi quy lỗi thuật toán. Đề xuất release gate: mọi pod Ready và mọi `imageID` khớp digest được phê duyệt; kiểm thử rollback bằng digest cũ và chính sách lưu giữ artifact.

## Authoritative references
- [Kubernetes Images](https://kubernetes.io/docs/concepts/containers/images/)
- [Kubernetes Deployments](https://kubernetes.io/docs/concepts/workloads/controllers/deployment/)
