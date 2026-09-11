# UNIT-DOTNET-010 — Admission Check Race

## Mục tiêu

Điều tra một admission-control gate giới hạn concurrency nhưng vẫn nhận nhiều công việc hơn capacity đã cấu hình trong một cửa sổ cạnh tranh nhỏ. Lab tập trung vào cách phân biệt observation với atomic state transition trong concurrent code.

## Bối cảnh thực tế

Một background export service chỉ được phép chạy tối đa một export nặng tại một thời điểm. Khi slot đã bận, request mới phải bị từ chối ngay thay vì xếp hàng. Ở tải bình thường hệ thống có vẻ đúng, nhưng khi hai request đến rất gần nhau, telemetry cho thấy đôi lúc cả hai đều được nhận và request thứ hai chỉ bắt đầu muộn hơn.

## Bạn cần làm gì

1. Chạy `reproduce.ps1` và ghi lại kết quả của hai request.
2. Ghi ít nhất hai hypothesis vào `workspace/my-investigation.md`.
3. Điều tra `starter/AdmissionGate.cs` và xác định vì sao policy "accept hoặc reject ngay" không được đảm bảo.
4. Sửa trực tiếp code trong `starter/`.
5. Chạy `verify.ps1` để chứng minh chỉ một request được nhận khi capacity là `1`.
6. Sau đó mới mở reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell
- Không cần database, Docker hay cloud service

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/11/UNIT-DOTNET-010-admission-check-race"
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy hai export gần như đồng thời với capacity bằng `1`. Harness có một diagnostic observer để làm cửa sổ cạnh tranh xảy ra deterministic thay vì phụ thuộc scheduler may rủi.

## Những gì cần quan sát

- Có bao nhiêu request được báo `Accepted`.
- Request thứ hai có bị reject ngay hay chỉ hoàn thành muộn hơn request thứ nhất.
- Giá trị capacity được quan sát ở thời điểm nào so với thời điểm quyền sử dụng slot thực sự được lấy.

Không cần dựa vào timing tuyệt đối; lab kiểm tra contract bằng số lượng request accepted/rejected.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thu thập evidence từ control flow.
4. Thử fix trong `starter/`.
5. Verify.
6. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ mở sau khi đã reproduce và tự thử sửa.

- [Reference Solution](solution/README.md)

## Expected Results

### Starter

- Cả hai request có thể đi qua admission path.
- Cả hai cuối cùng được báo `Accepted` dù capacity là `1`.
- Công việc vẫn chạy tuần tự, nên bug có thể bị nhầm là "semaphore vẫn hoạt động đúng".

### Sau khi sửa

- Chính xác một request được `Accepted`.
- Request còn lại được `Rejected` ngay tại admission boundary.
- Không tăng capacity và không tạo queue ngầm.

Nếu reproduce không chạy, kiểm tra `dotnet --info` và chắc chắn .NET 8 SDK khả dụng.

## Estimated Time

30–50 phút.