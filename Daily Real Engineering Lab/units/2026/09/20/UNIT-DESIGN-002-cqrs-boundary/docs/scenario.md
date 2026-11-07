# Scenario

Module `WarehouseOperations` có 18 HTTP endpoints.

- 10 endpoints CRUD cho carrier mappings, dock configuration và reason codes. Business rules ít, write đơn giản, traffic thấp.
- 3 read endpoints cho dashboard. Query có projection riêng nhưng cùng SQL database.
- `ConfirmReceiving`: kiểm tra trạng thái receiving, cập nhật inventory, ghi audit record trong cùng transaction.
- `AllocateInventory`: có optimistic concurrency, nhiều validation rule và có thể thất bại do competing allocation.
- `CloseShipment`: cập nhật shipment, tạo outbox record để downstream notification xử lý sau commit.
- 2 admin actions hiếm dùng.

Hiện controller gọi application services trực tiếp. Một proposal yêu cầu chuyển toàn bộ 18 endpoints sang `IRequest<T>` + MediatR handler, mỗi endpoint một command/query, đồng thời thêm validation/logging behaviors. Proposal khác muốn giữ nguyên application services vì CQRS bị xem là overengineering.

## Constraints

- 4 backend developers.
- Release hàng tuần.
- Một SQL database; không có requirement separate read/write stores.
- p95 mục tiêu dưới 300 ms nhưng traffic hiện thấp.
- Production support do chính team xử lý.
- Audit và authorization bắt buộc cho write workflows.
- Không có kế hoạch microservices trong 12 tháng tới.
- Team muốn giảm regression khi business rules tăng, nhưng không muốn tăng ceremony cho CRUD đơn giản.

## Deliverable

Trong `workspace/my-decision.md`, hãy ghi:

1. workload classification;
2. ít nhất 3 alternatives;
3. decision và boundary cụ thể;
4. transaction/validation/authorization/observability ownership;
5. migration sequence;
6. risks;
7. measurable revisit triggers.