# UNIT-ARCH-003 — Chọn execution model cho reconciliation job

## Mục tiêu

Đưa ra quyết định kỹ thuật có căn cứ cho một workload reconciliation định kỳ: chạy bên trong ASP.NET Core bằng `BackgroundService`, tách thành worker service, hay dùng Azure Functions.

## Bối cảnh thực tế

Một hệ thống commerce cần đối soát trạng thái khoảng 200.000 bản ghi mỗi đêm với hệ thống đối tác. Job hiện chạy trong web application. Trong các đợt deploy hoặc scale-in, job đôi khi bị gián đoạn; đội vận hành cũng muốn giảm coupling giữa API traffic và batch workload. Tuy nhiên team nhỏ, ngân sách vận hành hạn chế và chưa muốn tạo thêm quá nhiều infrastructure.

## Bạn cần làm gì

1. Đọc constraints trong `docs/scenario.md`.
2. Ghi assumptions và decision criteria vào `workspace/my-decision.md`.
3. So sánh ít nhất 3 phương án.
4. Chọn một phương án cho hiện tại và nêu rõ điều kiện khiến bạn đổi quyết định sau này.
5. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

Không cần runtime hay cloud account. Đây là Design Decision Lab.

## Chạy nhanh

Không có bước execute bắt buộc. Bắt đầu từ `docs/scenario.md`.

## Cách reproduce vấn đề

Không áp dụng. Vấn đề là lựa chọn execution model dưới các ràng buộc production cụ thể.

## Những gì cần quan sát

- failure boundary khi process bị recycle hoặc deploy
- retry và checkpoint semantics
- resource isolation giữa API và batch
- operational complexity
- deployment topology
- cost và khả năng vận hành của team

## Quy tắc làm lab

1. Ghi assumptions trước.
2. Xác định decision criteria.
3. So sánh alternatives.
4. Chọn và bảo vệ quyết định.
5. Xem reference solution sau cùng.

## Hints

Không cần hint mặc định. Nếu bí, hãy bắt đầu từ failure boundary và ownership của workload.

## Reference Solution

> Reference Solution — inspect only after attempting your own decision.

[Reference Solution](solution/README.md)

## Expected Results

Một quyết định có thể bảo vệ được bằng constraints, không phải câu trả lời kiểu “dịch vụ cloud luôn tốt hơn” hoặc “BackgroundService luôn đơn giản hơn”.

## Estimated Time

45–60 phút.
