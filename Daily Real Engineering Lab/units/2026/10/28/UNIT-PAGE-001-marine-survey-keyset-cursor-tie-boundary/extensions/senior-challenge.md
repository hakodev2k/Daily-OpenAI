# Senior challenge — snapshot semantics
Khi quan sát mới có thể được thêm hoặc backfill giữa hai request, composite cursor một mình không đảm bảo một snapshot cố định. Hãy đề xuất contract `snapshotVersion` hoặc `highWaterMark`, mô tả hành vi khi dữ liệu bị sửa/xóa và cách index/transaction hỗ trợ. Không thay đổi scope fix chính trước khi verification pass.
