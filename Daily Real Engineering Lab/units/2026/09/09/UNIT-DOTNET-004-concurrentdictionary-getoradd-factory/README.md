# UNIT-DOTNET-004 — One Cache Entry, Two Expensive Initializations

## Mục tiêu

Điều tra một concurrency contract trong .NET nơi nhiều request cùng yêu cầu một tenant-scoped client. Hệ thống cuối cùng chỉ giữ một entry, nhưng telemetry cho thấy quá trình khởi tạo đắt đỏ có thể chạy nhiều hơn một lần.

## Bối cảnh thực tế

Một SaaS backend lưu `TenantClient` theo tenant để tái sử dụng connection/configuration đã khởi tạo. Sau khi traffic tăng, team thấy metric `tenant_client_created_total` đôi lúc tăng gấp đôi cho cùng một tenant trong vài millisecond, dù registry chỉ chứa một client.

Việc khởi tạo thực tế có thể bao gồm đọc certificate, dựng SDK client và đăng ký callback; duplicate initialization làm tăng latency và có thể tạo side effect ngoài ý muốn.

## Bạn cần làm gì

1. Chạy starter ở chế độ quan sát.
2. Chạy `reproduce.ps1` để chứng minh symptom dưới contention có kiểm soát.
3. Ghi ít nhất hai hypothesis trước khi mở hints.
4. Đọc code và xác định contract nào đang bị hiểu sai.
5. Sửa trực tiếp code trong `starter/` để việc khởi tạo thực sự chỉ xảy ra một lần cho cùng key, nhưng vẫn giữ thread-safety.
6. Chạy `verify.ps1`.
7. Sau khi pass, so sánh với reference solution và trade-offs.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7 hoặc Windows PowerShell
- Không cần database, Docker hay cloud service

## Chạy nhanh

```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Harness tạo hai caller đồng thời cho cùng tenant và dùng một contention gate có timeout để tăng tính deterministic. Script chỉ thành công khi symptom ban đầu được quan sát đúng.

## Những gì cần quan sát

- hai caller có nhận cùng một object hay không
- registry cuối cùng có bao nhiêu logical entry
- số lần expensive initialization thực tế chạy
- thread-safety của container có đồng nghĩa với exactly-once execution của callback hay không

README cố ý không chỉ ra API contract hoặc fix cụ thể. Hãy dựa vào evidence trước.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

> **Spoiler:** chỉ mở sau khi bạn đã reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Trước khi sửa:

- cả hai caller thường nhận cùng logical client
- dictionary chỉ giữ một entry cho tenant
- expensive initialization được quan sát nhiều hơn một lần dưới contention

Sau khi sửa:

- cả hai caller nhận cùng logical client
- dictionary vẫn thread-safe
- expensive initialization cho cùng tenant chỉ chạy một lần
- không cần serialize toàn bộ registry cho các tenant khác nhau

Nếu không reproduce được, chạy lại `./reproduce.ps1`; contention gate có timeout để tránh treo vô hạn trên máy chậm.

## Estimated Time

30–50 phút.
