# UNIT-MONGO-002 — Turbine bị điều phối sửa phanh dù kiểm tra phanh đạt

## Mục tiêu
Điều tra sự sai lệch giữa dữ liệu inspection lưu trong MongoDB và danh sách turbine được chuyển tới đội sửa chữa. Tái hiện lỗi, kiểm tra từng document, đề xuất giả thuyết, sửa query và xác minh bằng regression fixture.

## Bối cảnh thực tế
Một wind farm tự động tạo ticket khi turbine có inspection phanh thất bại. Dashboard vận hành hiển thị 5 turbine cần can thiệp, nhưng kỹ sư hiện trường đối chiếu phiếu inspection và chỉ xác nhận 3 turbine. Ticket sai làm tăng chi phí điều phối và có thể trì hoãn việc sửa các turbine thực sự lỗi.

## Bạn cần làm gì
1. Chạy baseline, ghi lại danh sách turbine và bằng chứng cho hai ticket bị nghi sai.
2. Đưa ra ít nhất hai hypothesis có thể kiểm chứng từ document và query.
3. Chỉ sửa `starter/query.js`; không sửa fixture, bộ kiểm tra hay kết quả mong đợi.
4. Chạy `verify.ps1`, xác nhận cả baseline và các regression documents bổ sung.
5. Sau khi tự giải quyết mới xem reference solution.

## Yêu cầu môi trường
Docker Engine và Docker Compose v2; PowerShell 7 hoặc Windows PowerShell 5.1. Docker image được pin ở `mongo:7.0.14`. Không cần MongoDB Atlas, cloud account hay dịch vụ trả phí. Không mở cổng database ra host.

## Chạy nhanh
```powershell
./setup.ps1
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
`REPRODUCTION_CONFIRMED` nghĩa là baseline gốc đã thể hiện sai lệch; không phải kết quả xác nhận fix. Script này chủ đích kiểm tra trạng thái starter chưa sửa.

## Những gì cần quan sát
- Danh sách turbine được query trả về so với danh sách đội vận hành xác nhận.
- Từng inspection entry của các turbine bị nghi ngờ.
- Mối liên hệ giữa các điều kiện lọc và dữ liệu thực tế.
- Sự khác nhau giữa một bản ghi có nhiều inspection và một bản ghi chỉ có một inspection.

Bằng chứng mô phỏng: [incident](evidence/incident.md). Ghi chép: [my-investigation](workspace/my-investigation.md).

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ đọc sau khi tự điều tra:** [solution/README.md](solution/README.md) · [reference query](solution/query.js).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
50 phút · L2 / D4 · Validation: static-review (chưa thực thi MongoDB container trong lượt tạo).
