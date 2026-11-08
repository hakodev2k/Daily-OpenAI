# Acceptance matrix (điền trước khi xem lời giải)

| Case | Input | Expected invariant |
|---|---|---|
| C1 | A generation 41 ghi trước khi hết hạn, không có generation mới | Ghi hợp lệ |
| C2 | B generation 42 commit, A generation 41 ghi muộn | A không thể thay thế output 42 |
| C3 | A đọc ownership ở đầu rồi bị pause trước commit | Quyền phải được kiểm tra tại điểm commit |
| C4 | Hai request cùng generation 42 retry cùng một output | Idempotent, không sinh bản mâu thuẫn |
| C5 | B upload object nhưng crash trước khi cập nhật DB | Không công bố output chưa được commit |
| C6 | Storage ghi file không hỗ trợ atomic transaction với DB | Cần staging/immutable object + atomic DB pointer |
| C7 | Clock worker lệch 15 giây | Quyết định quyền không dựa vào đồng hồ local worker |
| C8 | B hoàn thành nhưng message retry đến muộn | Retry không làm giảm generation đã công bố |

Hãy ghi với mỗi case: trạng thái trước, thao tác, kết quả, chứng cứ và metric/alert cần theo dõi.