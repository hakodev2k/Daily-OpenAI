# UNIT-HTTP-008 — Cross-tenant header bleed khi dùng shared HttpClient

## Mục tiêu

Điều tra một production incident trong đó hai request đồng thời tới cùng partner API đôi khi bị gắn sai credential tenant, dù từng request riêng lẻ đều hoạt động đúng.

## Bối cảnh thực tế

Một integration service dùng một `HttpClient` dùng chung để gọi partner API cho nhiều tenant. Production log cho thấy thỉnh thoảng tenant A nhận `401`, còn phía partner ghi nhận request của tenant A mang credential của tenant B. Lỗi rất khó thấy khi test tuần tự và xuất hiện rõ hơn khi có concurrent traffic.

## Bạn cần làm gì

- Chạy starter và reproduce incident.
- Thu thập output của từng request và so sánh tenant mong đợi với authorization thực tế gửi đi.
- Ghi ít nhất 2 hypothesis trước khi sửa.
- Sửa learner-editable code trong `starter/`.
- Chạy `verify.ps1` để chứng minh request context không còn bị lẫn giữa các tenant.
- Sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/14/UNIT-HTTP-008-shared-default-request-headers"
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Starter dùng một fake HTTP handler và synchronization gate để tái hiện concurrent request deterministically, không cần gọi dịch vụ bên ngoài.

## Những gì cần quan sát

- Hai logical request được tạo cho hai tenant khác nhau.
- Ít nhất một request có authorization không khớp với tenant mong đợi.
- Không cần exception để incident xảy ra: request vẫn được gửi thành công ở transport layer nhưng mang context sai.

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

> Reference Solution — chỉ xem sau khi đã reproduce và tự thử fix.

[Reference Solution](solution/README.md)

## Expected Results

**Before**
- concurrent run cho thấy request context của hai tenant có thể bị lẫn.

**After**
- mỗi request luôn mang đúng authorization tương ứng với tenant của chính request đó.
- vẫn tiếp tục reuse `HttpClient` thay vì tạo client mới cho từng call.

## Estimated Time

45–60 phút.
