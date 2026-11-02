# Reference Solution — SPOILER

**Chỉ xem sau khi đã reproduce và tự thử sửa.**

## 1. Symptoms

Chạy tuần tự tạo hai nhãn đúng. Chạy chồng thời gian khiến nhãn A chứa thông tin của lô B, dù gateway trả đúng phí và dữ liệu đầu vào không đổi.

## 2. Evidence

Harness khởi động A rồi B; cả hai đều dừng tại `await` phí. Gateway được giải phóng theo thứ tự B rồi A. Starter ghi `Sequential contract: PASS`, `Overlapping contract: FAIL`. Nhãn A lẫn header và fee của B.

## 3. Root cause

`LabelComposer` giữ một `StringBuilder` mutable làm field `_buffer`. Mỗi `BuildAsync` gọi `Clear()`, ghi header rồi `await`. Trong lúc A chờ, B xóa và ghi lên cùng buffer. Sau khi B hoàn thành, A tiếp tục append vào buffer mà B đã dùng. Một instance được chia sẻ cho nhiều lời gọi không bảo đảm mỗi invocation sở hữu state riêng. `async` không tự động làm mutable instance fields an toàn.

## 4. Why the fix works

Trong [Program.cs](Program.cs), tạo `var buffer = new StringBuilder();` **bên trong** `BuildAsync`. Local variable thuộc async state machine của từng invocation, không bị lời gọi khác xóa hoặc ghi đè. Không cần serialize hai yêu cầu; vẫn chờ gateway bất đồng bộ.

## 5. How to verify

~~~powershell
dotnet run --project ./solution/SeedBank.Labels.csproj --configuration Release -- --verify
~~~

Reference exit 0 với cả hai hợp đồng PASS. Sau khi tự sửa `starter/Program.cs`, chạy `./verify.ps1` và xác nhận exit 0. Kiểm tra nội dung từng nhãn, không chỉ mã thoát.

## 6. Alternative fixes

- Trả về dữ liệu dạng immutable record, sau đó format bằng hàm thuần cho từng yêu cầu: tốt khi hệ thống cần phân tách nghiệp vụ và presentation.
- Dùng `SemaphoreSlim` bảo vệ toàn bộ thao tác trên buffer chia sẻ: có thể bảo đảm tính đúng nếu lock bao cả thời gian `await`, nhưng biến mọi yêu cầu thành hàng đợi, giảm throughput và tăng tail latency. Cần đo đạc nếu chọn.

## 7. Wrong / tempting fixes

- Chỉ `Clear()` thêm một lần sau `await`: có thể xóa dữ liệu yêu cầu khác và không tạo ownership.
- Thêm `Task.Delay` hoặc tăng timeout: thay đổi lịch chạy, không loại bỏ shared-state race.
- Dùng `lock` chỉ quanh từng `Append`: các bước của hai nhãn vẫn có thể xen kẽ; một critical section ngắn không bảo vệ toàn bộ hợp đồng.
- Tạo thêm service instance ngẫu nhiên: có thể che lỗi trong test, nhưng không phải bảo đảm khi instance được dùng lại.

## 8. Production implications

Nhãn vận chuyển có thể rò rỉ mã lô và thông tin kho giữa các đơn vị. Đánh giá các singleton formatter, builder, buffer, collection và service field có thể bị truy cập từ nhiều request. Quan sát cả integrity và confidentiality.

## 9. Trade-offs

Per-invocation `StringBuilder` tạo allocation nhỏ nhưng loại bỏ contention và cô lập dữ liệu. Pooling chỉ đáng cân nhắc sau profiling và phải có ownership/lifetime contract rõ ràng. Tối ưu sớm bằng shared mutable buffer có thể đắt hơn rất nhiều so với allocation.

## 10. What a Senior engineer should notice

Sự cố không nằm ở `Task` hay gateway: boundary của dữ liệu mutable bị đặt sai. Test tuần tự PASS không chứng minh concurrency safety. Regression test phải ép interleaving có kiểm soát và xác nhận **đúng dữ liệu của từng request**, không chỉ rằng hai tác vụ hoàn thành.
