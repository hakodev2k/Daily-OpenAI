# Reference Solution

> Chỉ xem sau khi đã reproduce và tự thử fix.

## Symptoms

Batch lập kế hoạch đủ ba tác vụ nhưng khi chạy sau vòng lặp thì các delegate không còn truy cập đúng region của iteration đã tạo ra chúng.

## Evidence

`i` đã đạt giá trị terminal của vòng lặp trước khi bất kỳ delegate nào thực thi. Tất cả delegate cùng đóng trên một biến đó.

## Root cause

Lambda capture biến, không capture giá trị tại thời điểm lambda được tạo. Với biến điều khiển của `for`, các delegate được lưu lại đều đọc cùng một storage location khi chạy sau đó.

## Why the fix works

Tạo một local snapshot bên trong mỗi iteration rồi capture snapshot đó. Mỗi delegate khi escape khỏi iteration sẽ giữ state tương ứng với region của chính nó.

## How to verify

Chạy `verify.ps1`. Kết quả phải có đúng một dòng cho `north`, `central`, `south` và process exit code bằng 0.

## Alternative fixes

- Capture trực tiếp giá trị region vào một local variable per iteration thay vì index.
- Tránh lưu delegate nếu có thể thực thi công việc ngay trong iteration.
- Chuyển dữ liệu cần chạy thành object immutable, sau đó map object sang worker ở pha execute.

## Wrong or misleading fixes

- Bắt `IndexOutOfRangeException`: chỉ che symptom.
- Reset `i` trước khi chạy delegate: mọi delegate vẫn chia sẻ cùng state.
- Thêm delay: không thay đổi semantics của closure.

## Production implications

Closure capture sai có thể tạo lỗi khó thấy hơn exception, ví dụ tất cả queued work dùng cùng tenant ID hoặc partition key. Khi delegate/task chạy bất đồng bộ, khoảng cách thời gian giữa capture và execution làm bug khó liên hệ với vòng lặp ban đầu.

## Trade-offs

Snapshot local là fix nhỏ và rõ ràng. Với workflow phức tạp hơn, representation bằng immutable work item thường dễ test, log và retry hơn delegate chứa hidden captured state.

## What a Senior engineer should notice

Khi callback, lambda hoặc task escape khỏi scope tạo ra nó, hãy review captured state như một lifetime boundary. Câu hỏi quan trọng là state được capture có immutable/per-operation hay đang trỏ tới storage tiếp tục thay đổi.
