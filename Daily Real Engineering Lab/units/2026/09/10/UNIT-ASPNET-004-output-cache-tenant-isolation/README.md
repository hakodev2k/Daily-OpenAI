# UNIT-ASPNET-004 — Output Cache Tenant Isolation

## Mục tiêu

Điều tra một ASP.NET Core API trả dữ liệu đúng cho request đầu tiên nhưng có thể trả payload của tenant khác cho request kế tiếp dù endpoint đọc đúng `X-Tenant-Id`.

## Bối cảnh thực tế

Một internal feature-flags API phục vụ nhiều tenant. Sau khi bật output caching để giảm tải, support ghi nhận tenant B đôi khi nhận cấu hình giống tenant A vừa gọi trước đó. Database/fake source không có shared mutable state và log request vẫn ghi đúng tenant hiện tại.

## Bạn cần làm gì

1. Chạy starter và reproduce lỗi.
2. Ghi lại symptom và ít nhất 2 hypothesis trong `workspace/my-investigation.md`.
3. Xác định lớp nào có thể trả response mà không chạy lại endpoint logic.
4. Sửa `starter/` sao cho mỗi tenant chỉ nhận đúng response của chính tenant đó.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới so sánh với reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell
- Port `5187` đang trống

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script gửi liên tiếp hai GET request tới cùng endpoint:

- request 1 với `X-Tenant-Id: tenant-a`
- request 2 với `X-Tenant-Id: tenant-b`

## Những gì cần quan sát

- Header của hai request khác nhau.
- Request thứ hai vẫn có thể nhận `tenant` và `featurePlan` giống response đầu tiên.
- Endpoint không dùng static dictionary hoặc global variable để lưu response giữa hai request.
- Nếu process được restart rồi đổi thứ tự tenant, tenant gọi đầu tiên lại trở thành response được quan sát ở request sau.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ xem sau khi đã reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Before:

```text
FIRST_TENANT=tenant-a
SECOND_TENANT=tenant-a
SECOND_PLAN=Premium
REPRODUCED=true
```

After:

```text
FIRST_TENANT=tenant-a
SECOND_TENANT=tenant-b
SECOND_PLAN=Standard
VERIFY=passed
```

## Estimated Time

35–50 phút.
