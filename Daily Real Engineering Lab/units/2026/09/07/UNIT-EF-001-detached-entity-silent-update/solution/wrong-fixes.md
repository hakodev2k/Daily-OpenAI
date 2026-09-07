# Wrong Fixes

## Gọi `SaveChangesAsync` hai lần
Không tạo ra tracked change mới. Nếu lần đầu không có entity modified thì lần hai vẫn không có gì để persist.

## Chỉ log giá mới rồi coi là thành công
Log của object CLR chứng minh phép gán đã xảy ra, không chứng minh persistence store đã thay đổi.

## Tạo DbContext mới rồi gọi `SaveChangesAsync`
DbContext mới không tự biết detached object nào cần update nếu bạn không attach/load và đánh dấu thay đổi.

## Dùng `Update(entity)` cho mọi detached payload
Có thể làm lab pass nhưng dễ đánh dấu toàn entity modified, gây ghi đè field không chủ đích. Chọn tracking/attach strategy theo transaction boundary và patch semantics.
