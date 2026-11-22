# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms
Payload đầu tiên chuyển thành `BRAVO` dù được tạo từ `ALPHA`; lần encode thứ hai vẫn đúng.

## Evidence
`ReusablePool` tái sử dụng cùng mảng sau `Return`. `ReadOnlyMemory<byte>` chỉ giới hạn quyền ghi qua view, không đóng băng nội dung của backing array.

## Root cause
`DispatchEncoder.Encode` trả về view tham chiếu tới mảng đã trả về pool trong `finally`. Người gọi không sở hữu bộ nhớ; lần thuê sau ghi đè byte của payload đang được giữ.

## Why the fix works
Copy đúng 5 byte vào mảng riêng trước khi trả mảng thuê về pool: `return buffer.AsSpan(0, 5).ToArray();`. Bộ nhớ của kết quả không còn bị pool tái sử dụng.

## How to verify
Áp dụng thay đổi vào `starter/Program.cs`, chạy `./verify.ps1`; cả hai chuỗi phải đúng. Có thể chạy `dotnet run --project solution/DispatchLab.csproj` để so sánh riêng.

## Alternative fixes
Chuyển ownership qua một đối tượng disposable, chỉ trả buffer sau khi consumer hoàn tất; cần lifecycle rõ ràng, tránh double-return. Hoặc encode trực tiếp vào buffer do consumer sở hữu.

## Wrong / tempting fixes
`ReadOnlyMemory` không tạo immutable snapshot. Thêm `Task.Delay` hoặc lock quanh Encode không bảo vệ sau khi method trả về. Tắt pooling có thể che triệu chứng nhưng không giải quyết ownership contract.

## Production implications
Hàng đợi async và network send có thể đọc byte muộn hơn producer. Payload bị sửa có thể gây sai đơn hàng, chữ ký số hoặc checksum.

## Trade-offs
Copy phát sinh allocation và memory bandwidth; lease/ownership giảm copy nhưng phức tạp hơn và phải được quản lý qua mọi nhánh lỗi, cancellation.

## Senior engineering observation
Đánh giá lifetime của memory và contract của producer/consumer; thêm regression test cho delayed consumption và reuse, không chỉ assert ngay sau encode.
