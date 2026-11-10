# UNIT-CUSTODY-001 — Chuỗi bảo quản bằng chứng cứu hộ offline

## Mục tiêu
Thiết kế và kiểm chứng tính toàn vẹn, truy vết và xuất bản bằng chứng trong môi trường thiết bị mất kết nối.

## Bối cảnh thực tế
Ba điện thoại ghi ảnh hiện trường cứu hộ. Một điện thoại offline 9 giờ. Hai bản export của cùng vụ việc có digest khác nhau; kiểm toán yêu cầu giải thích.

## Bạn cần làm gì
Đọc dữ liệu tại evidence/, ghi ba giả thuyết, đề xuất ADR vào workspace/decision.md, kiểm tra từng case trong docs/verification-matrix.md. Đừng xem solution trước khi hoàn tất.

## Yêu cầu môi trường
Git và Markdown editor; không cần dịch vụ cloud.

## Chạy nhanh
Chạy `git pull`, mở evidence/*.csv, điền workspace/decision.md.

## Cách reproduce vấn đề
So sánh E-01 và E-02 trong artifacts.csv, dựng lại thứ tự sự kiện từ timeline.csv; ghi rõ những gì chưa thể kết luận.

## Những gì cần quan sát
Cùng mã vụ việc nhưng export digest khác nhau; log không chứng minh được thứ tự sửa đổi hay nguồn tin cậy.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix qua bản thiết kế.
4. Verify bằng ma trận đối kháng.
5. Chỉ sau đó mới xem solution.

## Hints
[1](hints/hint-01.md) · [2](hints/hint-02.md) · [3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference](solution/README.md)

## Expected Results
ADR với trust boundaries, state machine, export protocol và kết quả V1–V8.

## Estimated Time
55 phút.
