# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms

Caller nhận `TimeoutException` sau khoảng 80 ms, nhưng operation chậm vẫn tồn tại và sau đó ghi vào fixture dùng bởi scenario kế tiếp.

## 2. Evidence

Timeline quan trọng là: timeout được quan sát trước khi task công việc kết thúc. Sau test boundary, queue vẫn nhận `late-write`.

## 3. Root cause

Timeout wrapper chỉ dừng việc **chờ** operation bằng `Task.WhenAny`; nó không hủy operation và cũng không đảm bảo operation đã kết thúc trước khi trả control cho caller. Task bị mất ownership trở thành orphan work đối với test harness.

## 4. Why the fix works

Reference solution biến timeout thành một cancellation boundary có ownership rõ ràng: token được truyền xuống operation; timeout yêu cầu cancellation; wrapper chỉ hoàn tất sau khi operation đã quan sát cancellation và kết thúc. Vì vậy không còn task cũ ghi state sau test boundary.

## 5. How to verify

Copy cách sửa tương đương vào `starter/Program.cs`, sau đó chạy:

```powershell
./verify.ps1
```

Kết quả cần có `NO_ORPHANED_WORK` và process exit code 0.

## 6. Alternative fixes

- Nếu dependency không hỗ trợ cancellation, isolate state/resource theo từng test để orphan work không thể ghi vào fixture của test khác, đồng thời theo dõi task và await cleanup trước teardown.
- Với API hỗ trợ timeout native, ưu tiên contract native nhưng vẫn phải xác minh lifecycle của operation sau timeout.

## 7. Wrong / Tempting Fixes

- Tăng timeout chỉ làm race khó thấy hơn.
- Thêm `Task.Delay` trước test kế tiếp che triệu chứng nhưng không tạo ownership.
- Bỏ shared fixture có thể giảm flakiness nhưng không giải quyết resource leak nếu operation vẫn tiếp tục chạy.
- Chỉ assert `TimeoutException` là chưa đủ: test phải kiểm tra hậu điều kiện sau timeout.

## 8. Production implications

Cùng pattern trong production có thể tạo request đã timeout nhưng downstream work vẫn chạy, tiếp tục giữ connection, ghi dữ liệu hoặc tạo side effect sau khi caller đã bỏ cuộc.

## 9. Trade-offs

Cancellation là cooperative. Code phải quyết định operation nào an toàn để cancel, cleanup nào bắt buộc, và khi nào cần isolation thay vì giả định cancellation sẽ dừng tức thì.

## 10. What a Senior engineer should notice

Timeout là policy về thời gian chờ; cancellation và task ownership là lifecycle contract. Một test tốt không chỉ kiểm tra caller nhận timeout mà còn kiểm tra công việc phía sau đã đi vào trạng thái an toàn.
