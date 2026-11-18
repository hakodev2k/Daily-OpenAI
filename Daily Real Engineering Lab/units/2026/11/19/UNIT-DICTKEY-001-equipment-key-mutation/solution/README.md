# Reference Solution — chỉ xem sau khi điều tra

## Root cause
Dictionary chọn bucket theo hash tại thời điểm insert. EquipmentKey.GetHashCode phụ thuộc AssetTag có setter; sau khi mutate, lookup tính hash mới và tìm ở bucket khác. Count không giảm vì entry cũ vẫn tồn tại. Không được kỳ vọng Dictionary tự rehash khi object key thay đổi.

## Cách sửa
Tách identity bất biến (`record EquipmentKey`) khỏi thông tin thiết bị có thể cập nhật (`DeviceRecord.CurrentAssetTag`). Dùng khóa được chốt khi đăng ký; cập nhật metadata trên value. Chạy `dotnet run --project solution/Registry.csproj` để xem kết quả mong đợi.

## Wrong fixes
Gọi `ContainsKey` trước khi mutate không bảo vệ lookup về sau. Chỉ override Equals mà không đồng bộ GetHashCode vi phạm contract. Quét toàn bộ Dictionary bằng reference equality che lỗi, tăng chi phí O(n) và không giải quyết identity.

## Trade-offs
Nếu đổi asset tag đồng nghĩa thay identity nghiệp vụ, thực hiện remove bằng **khóa cũ trước khi mutate**, kiểm tra collision và insert bằng khóa mới trong một thao tác đồng bộ có kiểm soát. Với cập nhật đồng thời, cần khóa/transaction bao quanh thay đổi; Dictionary thường không thread-safe.

## Verification
Trường hợp regression: thay nhãn nhiều lần, hai thiết bị cùng nhãn ở site khác nhau, lookup bằng key value-equal mới, xóa bản ghi, và collision khi rename. So sánh invariant count với truy xuất thực tế.
