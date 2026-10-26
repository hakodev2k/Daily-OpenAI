# Hồ sơ thay đổi — Search schema v2

## Business

Marketplace tìm nhà thuê ngắn hạn. Người dùng tìm theo vị trí, tiện nghi, giá và trạng thái còn phòng. Index hiện tại (v1) không có biểu diễn tiện nghi chuẩn hóa và geo filter mới; ứng dụng phải chuyển sang schema v2 trong 10 ngày.

## Workload hiện tại

| Chỉ tiêu | Giá trị |
|---|---:|
| Listing còn hoạt động | 18,000,000 |
| Tìm kiếm | 1,800 requests/s giờ cao điểm |
| Listing update/create/delete | 350 events/s trung bình |
| Backfill v2 thực đo | 900 documents/s |
| CDC replay v2 thực đo | 500 events/s |
| Search p95 SLO | <= 250 ms |
| Listing freshness sau cutover | <= 90 giây |
| Search availability mục tiêu | >= 99.95% |
| CDC retention | 72 giờ |
| Dung lượng v1 | 58 GB |
| Dung lượng v2 dự kiến | 72 GB |
| Storage capacity đã cấp | 140 GB |
| Chi phí tạm thời | tối đa 2.5x hiện tại, <= 48 giờ |
| Rollback mục tiêu | <= 10 phút |
| Đội trực | 2 engineers + 1 product owner |

**Source of truth**: PostgreSQL; search index chỉ là read model. Có change stream bền vững với event ID, entity ID, entity version và tombstone. Replay có thể redeliver, không bảo đảm exactly-once.

## Bất biến phải giữ

- Không hiển thị listing đã xóa/đã đóng chỉ vì sự kiện đến muộn.
- Không làm mất các cập nhật xảy ra trong lúc backfill.
- Truy vấn v1 vẫn hoạt động cho tới khi có quyết định cutover.
- Quyền hiển thị listing theo tenant/owner phải được kiểm tra đúng với schema v2.
- Không giả định `document count` bằng nhau chứng minh hai index tương đương.
- Rollback phải chỉ rõ schema nào đọc được và dữ liệu nào vẫn được cập nhật.
- Không dùng dịch vụ cloud trả phí để thực hiện lab; thiết kế production chỉ là phương án.

## Yêu cầu đầu ra

Hoàn thành `workspace/my-adr.md` với ít nhất ba phương án và một lựa chọn. Phải có sơ đồ bằng text, tính toán capacity/catch-up, các gates định lượng, canary, xử lý delete và update out-of-order, kế hoạch rollback, giới hạn chi phí, trách nhiệm triển khai.

## Requirement change (chỉ đọc sau khi chọn phương án ban đầu)

Vào ngày triển khai, chiến dịch quảng cáo làm tốc độ thay đổi tăng lên **600 events/s trong 4 giờ**; replay capacity không đổi. Hãy cập nhật quyết định: hoãn cutover, tăng năng lực hay đổi chiến lược? Chứng minh bằng phép tính, không chỉ nói “scale out”.
