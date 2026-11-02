# Reference Solution — chỉ đọc sau khi tự điều tra

## 1. Symptoms
HTTP 200 và CSV hợp lệ, nhưng caller gặp ObjectDisposedException khi đọc stream vừa nhận. Baseline cho thấy stream đã bị dispose trước thao tác đọc đầu tiên.

## 2. Evidence
TrackingHandler trả về StreamContent chứa TrackedStream. Trong baseline, IsDisposed đã true ngay sau OpenExportAsync. Đây là bằng chứng vòng đời tài nguyên, không phải lỗi CSV hoặc lỗi HTTP status.

## 3. Root cause
Starter đặt HttpResponseMessage trong using var bên trong OpenExportAsync rồi trả ra stream của HttpContent. Khi method kết thúc, response bị dispose; content sở hữu và dispose stream. Caller nhận tham chiếu tới một stream không còn sống.

## 4. Why the fix works
Bản tham chiếu giữ HttpResponseMessage sống cùng stream được trả ra bằng ResponseOwnedStream. Caller đóng wrapper thì wrapper đóng response, và response đóng content stream. Nếu status không thành công hoặc việc mở content thất bại, catch đóng response ngay. Không buffer toàn bộ CSV.

## 5. How to verify
Chạy trên unit gốc:

~~~powershell
./scripts/reproduce.ps1
./scripts/verify.ps1
dotnet run --project solution/SolutionProbe.csproj -- verify
~~~

Baseline reproduce PASS và verify FAIL. Bản reference verify PASS. Khi đã tự sửa starter/ExportClient.cs, verify trên starter phải PASS; reproduce không còn quan sát được lỗi cũ.

## 6. Alternative fixes
- Nếu caller chỉ cần các bản ghi đã parse, đọc/parse **bên trong** scope của response rồi trả kết quả; phù hợp khi dữ liệu nhỏ hoặc API không cần stream.
- Có thể trả IAsyncEnumerable<T> với response nằm trong vòng đời của async iterator; phải kiểm tra disposal khi consumer ngừng enumerate giữa chừng.
- Trả một đối tượng chuyên biệt có Stream và IDisposable/IAsyncDisposable, công khai ownership; cần thay đổi public API.

## 7. Wrong / tempting fixes
- Xóa using var response rồi trả raw stream: tránh lỗi tức thời nhưng rò rỉ tài nguyên khi caller không sở hữu response.
- Đọc hết body vào MemoryStream: có thể hợp lệ với file nhỏ, nhưng trái ràng buộc streaming và tạo memory pressure khi file lớn.
- Retry 200 OK: không giải quyết lỗi vòng đời; chỉ tăng request.
- Bỏ EnsureSuccessStatusCode: có thể biến trang lỗi HTTP thành dữ liệu CSV giả.

## 8. Production implications
Khi streaming, HTTP connection có thể bị giữ cho tới khi caller dispose stream. Phải dùng await using / using đúng scope, có timeout và cancellation phù hợp, đo số connection đang hoạt động và giới hạn concurrency. Không dùng wrapper để che việc caller quên dispose.

## 9. Trade-offs
Wrapper giữ nguyên Task<Stream> và streaming nhưng thêm code ủy quyền Stream, dễ thiếu API khi mở rộng. Async iterator có thể diễn đạt ownership tự nhiên hơn nhưng thay đổi contract. Buffering đơn giản hơn nhưng tốn RAM. Chọn theo kích thước payload, API contract và khả năng kiểm soát consumer.

## 10. What a Senior engineer should notice
HTTP 200 chỉ chứng minh headers/status được nhận. Cần xem ai sở hữu tài nguyên xuyên qua ranh giới method, nhất là khi trả Stream, DbDataReader hoặc IAsyncEnumerable. Regression test phải kiểm tra cả successful read, non-success cleanup, cancellation và disposal, không chỉ assert status.

## Authoritative references
- [HttpClient.SendAsync](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.sendasync)
- [HttpCompletionOption.ResponseHeadersRead](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpcompletionoption)
- [HttpResponseMessage.Dispose](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage.dispose)
- [Stream.DisposeAsync](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream.disposeasync)
