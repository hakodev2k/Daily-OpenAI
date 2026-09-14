# Reference Solution

> Chỉ xem sau khi bạn đã tự đưa ra quyết định.

## Một phương án defensible

Với constraints hiện tại, chọn **audit/history tables riêng trong cùng SQL Server database**, tách rõ current-state model và audit model, có retention/purge policy độc lập.

Đây không phải kiến trúc duy nhất đúng. SQL temporal tables hợp lý khi cần history gần với relational row history và team muốn giảm application-level plumbing. Append-only store riêng phù hợp hơn khi audit volume, query pattern hoặc compliance boundary lớn đến mức cần operational lifecycle độc lập.

## Evidence từ constraints

- Current-state queries không cần 7 năm lịch sử.
- Audit data tăng nhanh và có retention riêng.
- Team nhỏ, chưa có data platform team.
- SQL Server đã là operational capability hiện có.
- Audit chủ yếu phục vụ support/compliance, không nằm trên hot request path.

## Root architectural concern

Current transactional state và long-lived audit history có **khác lifecycle, query pattern và retention semantics**. Gộp chúng thành cùng một model dễ tạo coupling về schema, indexes, purge và operational maintenance.

## Vì sao audit tables riêng phù hợp

- Giữ current-state schema gọn.
- Query audit vẫn dùng tooling SQL quen thuộc.
- Có thể partition/index/purge theo policy riêng.
- Application có thể ghi audit trong cùng transaction khi yêu cầu consistency cao.
- Chưa cần thêm một datastore/platform mới.

## Alternatives

### SQL temporal tables

Tốt khi cần row-version history tương đối tự động. Cần đánh giá history table growth, retention cleanup, schema changes và việc temporal history có đủ semantic context như actor/reason hay không.

### Append-only store riêng

Tách lifecycle mạnh nhất và scale độc lập, nhưng tăng ingestion, consistency, operations và query tooling. Chưa cần nếu volume/constraints hiện tại chưa biện minh complexity đó.

## Wrong / Tempting Decisions

- Nhét mọi phiên bản vào `Orders` và thêm cột `IsCurrent` mà không thiết kế query/index/purge.
- Chọn event sourcing chỉ vì cần audit log.
- Tách datastore ngay lập tức chỉ vì retention dài.
- Dùng temporal tables rồi giả định chúng tự động chứa đầy đủ business actor/reason.

## Production Implications

Cần audit write contract rõ ràng, index theo entity/time, retention job có telemetry, và schema version hoặc payload contract để lịch sử cũ vẫn đọc được khi model tiến hóa.

## Trade-offs

Audit tables cùng database đơn giản hơn về operations nhưng vẫn chia sẻ blast radius và storage với transactional database. Khi audit volume hoặc compliance isolation tăng mạnh, boundary này có thể cần tách tiếp.

## Senior Insight

Đừng bắt đầu từ “temporal table hay NoSQL”. Hãy tách các concerns: **source of truth hiện tại, lịch sử bất biến, retention, query patterns, consistency requirement, schema evolution và operational ownership**. Sau đó mới chọn storage pattern.
