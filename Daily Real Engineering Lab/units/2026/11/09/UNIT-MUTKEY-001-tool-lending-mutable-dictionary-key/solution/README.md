# Reference Solution — chỉ xem sau khi tự thử

## Symptoms
`Count=1`, lookup trước đổi nhãn đúng, sau đổi nhãn sai.

## Evidence
`Dictionary<TKey,TValue>` phân vùng khóa theo hash tại thời điểm insert; sau khi thuộc tính ảnh hưởng hash thay đổi, lookup tính hash mới.

## Root cause
`ToolAsset.GetHashCode()` và `Equals()` dựa vào `DisplayCode` có thể thay đổi. Khóa không còn giữ hợp đồng ổn định khi nằm trong Dictionary.

## Why fix works
Dùng `AssetId` bất biến làm khóa `Dictionary<string,string>`, tách định danh khỏi nhãn hiển thị. Mọi lookup sử dụng cùng identity.

## Verify
So sánh `dotnet run --project starter/Lab.csproj` trước/sau chỉnh sửa; `scripts/verify.ps1` phải exit 0. Có thể chạy `dotnet run --project solution/Lab.csproj` để đối chiếu.

## Alternative fixes
Một immutable record key chỉ chứa AssetId cũng phù hợp; custom comparer chỉ dùng AssetId là phương án khác nhưng dễ tạo coupling giữa entity và index.

## Wrong / tempting fixes
Reinsert khóa sau mỗi lần sửa nhãn chỉ xử lý một code path, dễ bỏ sót mutation khác. Dùng `ReferenceEqualityComparer.Instance` chỉ phù hợp nếu hợp đồng tra cứu luôn dựa trên cùng object instance, không phải asset identity qua request/process.

## Production implications
Identity nên bất biến, có constraint duy nhất trong persistent store; đồng bộ thay đổi entity/index trong cùng transaction hoặc áp dụng rebuild có kiểm soát.

## Trade-offs
String key đơn giản, dễ serialize, nhưng cần chuẩn hóa quy tắc case và scope tenant. Immutable value-object key tăng type safety nhưng thêm mapping.

## Senior engineer observation
`Count` không chứng minh khả năng lookup; test phải cover mutation, reconstruct object, và restart/rehydration.
