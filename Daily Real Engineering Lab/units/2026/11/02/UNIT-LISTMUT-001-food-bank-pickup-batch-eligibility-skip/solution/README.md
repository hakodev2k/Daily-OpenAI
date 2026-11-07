# Reference Solution — chỉ xem sau khi tự thử

## 1. Symptoms

Lô không đủ điều kiện đôi khi vẫn xuất hiện sau một lần `Prepare`, đặc biệt khi hai hoặc nhiều lô bị loại đứng liền nhau. Batch xen kẽ có thể khiến lỗi bị bỏ qua trong kiểm thử thông thường.

## 2. Evidence

Với input `FB-101, FB-102, FB-103, FB-104`, cả `FB-101` và `FB-102` đều quá hạn. Starter trả `FB-102, FB-103, FB-104`. `verify.ps1` thất bại tại contract "adjacent expired lots". Lần chạy thứ hai có thể làm lỗi biến mất, nhưng không thể coi đó là hành vi hợp lệ.

## 3. Root cause

Starter duyệt danh sách từ trái sang phải, xóa phần tử tại chỉ số hiện tại, rồi tăng chỉ số. Khi `RemoveAt(i)` dồn phần tử sau vào `i`, vòng lặp nhảy sang `i+1` và bỏ qua phần tử vừa được dồn vào. Một nhóm các lô không hợp lệ liên tiếp có thể còn sót.

## 4. Why the fix works

Xem [source tham chiếu](DonationDispatch.cs). `List<T>.RemoveAll(predicate)` lọc tất cả phần tử không hợp lệ trong một lần xử lý và giữ thứ tự tương đối của phần tử còn lại. Việc so sánh `BestBefore < pickupDate` bảo toàn trường hợp đúng ngày hạn.

Để áp dụng, chỉ thay phần thân `Prepare` trong `starter/DonationDispatch.cs` bằng logic tương ứng; không sửa `checks/`.

## 5. How to verify

Chạy `./verify.ps1` sau khi sửa starter. Harness tham chiếu trực tiếp project `starter/Dispatch.csproj`, không tham chiếu code trong `solution/`. Các contract gồm: lô hết hạn liền nhau, toàn bộ lô không hợp lệ, xen kẽ, quarantine, đúng ngày hạn, batch rỗng và gọi lại.

## 6. Alternative fixes

- Duyệt ngược từ `Count - 1` về 0 rồi `RemoveAt(i)`: đúng về mặt vị trí, nhưng với `List<T>` nhiều lần dồn phần tử có thể làm chi phí xấu nhất tăng lên O(n²).
- Duyệt xuôi nhưng không tăng `i` khi vừa xóa: đúng về logic, song vẫn có chi phí dịch chuyển nhiều lần.
- `Where(...).ToList()`: có thể đúng nếu contract cho phép trả danh sách mới, nhưng **không** đáp ứng yêu cầu in-place và identity của batch trong lab này.

## 7. Wrong / tempting fixes

- Chạy filter hai lần: chỉ che lỗi với một số cấu hình; nhóm dài có thể vẫn còn sót và gây thêm công việc.
- Bổ sung điều kiện `BestBefore <= pickupDate`: làm thay đổi chính sách nghiệp vụ, loại sai lô còn hạn đúng ngày.
- Sắp xếp danh sách trước khi lọc: thay đổi thứ tự ưu tiên pickup, có thể ảnh hưởng kế hoạch tài xế và không sửa đúng nguyên nhân.
- Chỉ tăng logging: có ích cho quan sát, nhưng không bảo đảm kết quả.

## 8. Production implications

Lỗi logic deterministic này có thể biến thành sai lệch phân bổ xe, chi phí vận chuyển và kiểm soát chất lượng. Cần test dữ liệu có phần tử không hợp lệ liên tiếp, toàn bộ không hợp lệ và boundary date; dashboard không nên coi batch count là bằng chứng eligibility. Không dùng collection đang được thread khác chỉnh sửa đồng thời; nếu có, phải thiết kế snapshot/ownership riêng.

## 9. Trade-offs

`RemoveAll` rõ ý định, xử lý tuyến tính O(n), dùng thêm rất ít bộ nhớ và bảo toàn thứ tự. Nếu caller không sở hữu list hoặc cần audit toàn bộ đầu vào, cách tạo danh sách mới có thể phù hợp hơn, nhưng cần thay đổi contract và tests một cách có chủ đích.

## 10. What a Senior engineer should notice

Một test chỉ có một phần tử lỗi hoặc các phần tử lỗi xen kẽ chưa đủ. Khi mutation xảy ra trong vòng lặp, phải xem xét invariant của chỉ số và tập phần tử chưa duyệt; xác định ownership của input; phân biệt chính sách nghiệp vụ với lỗi cấu trúc dữ liệu; kiểm chứng tính đúng đắn thay vì chỉ dựa vào số lượng hoặc retry.
