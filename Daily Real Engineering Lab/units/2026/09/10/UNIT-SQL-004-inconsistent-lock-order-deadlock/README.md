# UNIT-SQL-004 — Inventory Transfer Concurrency Design

## Mục tiêu

Đưa ra một quyết định concurrency design có thể bảo vệ correctness và availability cho luồng inventory transfer khi hai request cập nhật cùng một cặp location theo hướng ngược nhau.

## Bối cảnh thực tế

Một inventory service trên SQL Server xử lý chuyển quantity giữa các location. Trong production, hai request ngược chiều đôi khi tạo circular lock dependency và SQL Server phải hủy một transaction để hệ thống tiếp tục. Business muốn giảm lỗi nhưng không chấp nhận giải pháp chỉ retry vô hạn hoặc serialize toàn bộ hệ thống.

## Bạn cần làm gì

1. Đọc schema trong `starter/setup.sql` và evidence trong `evidence/incident.md`.
2. Xác định invariant cần giữ và failure mode cần loại bỏ.
3. So sánh ít nhất ba phương án concurrency control.
4. Chọn một phương án primary và mô tả transaction boundary, lock/order rule, retry policy nếu có, observability và rollout plan.
5. Ghi quyết định vào `workspace/my-decision.md` trước khi xem solution.

## Yêu cầu môi trường

Không bắt buộc chạy SQL Server. Nếu có SQL Server LocalDB hoặc SQL Server dev instance, bạn có thể dùng schema kèm theo để tự dựng thêm reproduction riêng.

## Chạy nhanh

Không có executable reproduction bắt buộc cho lab này. Bắt đầu từ:

```text
starter/setup.sql
→ evidence/incident.md
→ workspace/my-decision.md
```

## Cách reproduce vấn đề

Lab sử dụng captured incident evidence thay vì một timing-sensitive local deadlock reproduction. Hãy dựng wait-for graph từ timeline và lock evidence được cung cấp, sau đó xác định điều kiện tạo cycle.

## Những gì cần quan sát

- Mỗi request riêng lẻ đều hợp lệ.
- Hai request cạnh tranh cùng hai resources nhưng không tuân theo cùng một acquisition order.
- Correctness của stock total vẫn phải được giữ khi một transaction bị rollback.
- Giải pháp phải cân bằng throughput, latency, implementation risk và operational complexity.

## Quy tắc làm lab

1. Đọc evidence trước.
2. Ghi ít nhất ba options.
3. Chọn option dựa trên constraints, không dựa trên độ “xịn” của công nghệ.
4. Nêu failure modes còn lại và cách monitor.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Reference Solution — inspect only after completing your own decision record.

- [Reference Solution](solution/README.md)

## Expected Results

Một đáp án tốt phải giữ inventory invariant, loại bỏ hoặc giới hạn rõ circular-wait condition, không biến retry thành primary correctness mechanism, và có rollout/verification plan đo được.

## Estimated Time

45–75 phút.
