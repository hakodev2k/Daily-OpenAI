# UNIT-PROXYTRUST-001 — Cổng cấp phép đô thị: IP audit sai sau khi đổi reverse proxy

## Mục tiêu
Điều tra sự sai lệch giữa IP ghi trong audit và IP được network edge quan sát, rồi đề xuất quyết định triển khai có thể kiểm chứng trên nhiều môi trường.

## Bối cảnh thực tế
Portal xử lý hồ sơ cấp phép xây dựng chạy ASP.NET Core sau CDN và ingress. Sau đợt đổi topology, một số hồ sơ có địa chỉ IP audit không khớp với edge logs; rate limiting cũng nhóm nhầm người dùng. Nhóm vận hành chưa thống nhất liệu lỗi đến từ CDN, ingress, ứng dụng hay quy trình triển khai.

## Bạn cần làm gì
1. Đọc dữ liệu trong `evidence/incident.md`, phân biệt observation và assumption.
2. Tự dựng bảng nguồn tin cậy và đường đi của từng request trong `workspace/my-investigation.md`.
3. Đề xuất phương án xử lý, test matrix, rollout/rollback và các tín hiệu giám sát.
4. Chỉ sau khi tự làm, so sánh với `solution/README.md`.

## Yêu cầu môi trường
Không cần Azure, Docker hoặc package trả phí. Markdown editor là đủ. Có thể dùng PowerShell để chạy kiểm tra worksheet.

## Chạy nhanh
```powershell
Get-Content ./evidence/incident.md
Copy-Item ./workspace/my-investigation.md ./my-investigation.md
```

## Cách reproduce vấn đề
Tái hiện *logic* incident bằng cách lần theo ba request trong evidence, điền từng hop và xác định kết quả audit/rate-limit kỳ vọng theo trust policy mà bạn đề xuất. Không có production endpoint để gọi và không khẳng định đã chạy ứng dụng thực tế.

## Những gì cần quan sát
So sánh edge peer address, forwarding headers và giá trị IP được ứng dụng ghi. Xác định request nào cần bị từ chối hoặc bỏ qua thông tin chưa đủ tin cậy.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Xem hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Chỉ xem sau khi tự điều tra](solution/README.md).

## Expected Results
Một quyết định có trust boundary rõ ràng, ma trận kiểm thử cho spoofing và thay đổi topology, kế hoạch rollback, audit attribution nhất quán. Không được dựa vào một header tự khai báo mà chưa chứng minh nguồn.

## Estimated Time
65 phút.