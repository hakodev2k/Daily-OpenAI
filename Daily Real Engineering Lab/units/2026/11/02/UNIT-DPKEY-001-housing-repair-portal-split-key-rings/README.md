# UNIT-DPKEY-001 — Housing repair portal: phiên đăng nhập lỗi khi chuyển replica

## Mục tiêu
Tái hiện lỗi phiên đăng nhập chỉ xuất hiện khi traffic đi qua replica khác; phân tích evidence, đánh giá phương án vận hành key material và bảo vệ ranh giới ứng dụng.

## Bối cảnh thực tế
Cổng tiếp nhận yêu cầu sửa chữa nhà ở của thành phố chạy hai replica sau load balancer. Người dùng đăng nhập thành công, nhưng một phần request kế tiếp bị coi là phiên không hợp lệ, nhất là sau rolling deployment. CPU và database ổn định. Lab mô phỏng cơ chế bảo vệ ticket bằng ASP.NET Core Data Protection thật, không yêu cầu cloud.

## Bạn cần làm gì
1. Reproduce và ghi nhận năm tín hiệu: local, crossReplica, restart, otherAppRejected, tamperRejected.
2. Lập ít nhất ba giả thuyết: đồng bộ thời gian, deployment/session affinity, hoặc cơ chế xử lý ticket. Chọn evidence để phân biệt.
3. Chỉnh sửa code trong starter/ sao cho tất cả hợp đồng verify đều đạt.
4. Viết ADR ngắn: lựa chọn cho hai replica, key persistence, application isolation, rotation và bảo vệ key at rest.
5. Đề xuất canary verification và rollback khi triển khai thật.

## Yêu cầu môi trường
Windows PowerShell 5.1+ hoặc PowerShell 7; .NET 8 SDK và ASP.NET Core shared framework. Không cần tài khoản Azure, Docker hay package NuGet ngoài. global.json pin SDK 8.0.100 với latestFeature roll-forward.

## Chạy nhanh
    ./setup.ps1
    ./run.ps1
    ./reproduce.ps1

## Cách reproduce vấn đề
Chạy ./reproduce.ps1 trên starter nguyên bản. Exit code 0 nghĩa là **đã thấy triệu chứng**, không phải đã sửa xong. Sau khi sửa starter/, chạy ./verify.ps1. Nếu đã sửa thành công, reproduce có thể không còn đạt điều kiện lỗi ban đầu — đây là hành vi mong đợi.

## Những gì cần quan sát
So sánh ticket do replica A phát hành khi được xử lý tại A, B và A sau restart. Quan sát tính cách ly giữa hai ứng dụng và khả năng phát hiện ticket bị sửa đổi. Không in ticket hay key vào log. Mỗi lần chạy sử dụng thư mục tạm độc lập.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào workspace/my-investigation.md.
3. Thử fix trong starter/.
4. Verify bằng ./verify.ps1 (kiểm tra code bạn đã sửa).
5. Chỉ sau đó xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference explanation](solution/README.md) và [Reference code](solution/Program.cs). Không xem trước khi tự thử.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). Xem [decision brief](docs/decision-brief.md) để biết ràng buộc vận hành.

## Estimated Time
70 phút — L5 Design Decision / D4.
