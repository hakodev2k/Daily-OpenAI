# UNIT-DI-002 — Hai singleton không còn là cùng một singleton

## Mục tiêu

Điều tra một lỗi dependency graph trong .NET Dependency Injection khi cùng một registration set vô tình tạo ra nhiều object graph độc lập.

## Bối cảnh thực tế

Một internal routing service giữ cấu hình route trong memory. Operations cập nhật route thành công qua một component, nhưng request path khác vẫn dùng giá trị cũ. Log cho thấy cả hai component đều nói rằng chúng đang dùng `FeatureCatalog` singleton.

## Bạn cần làm gì

1. Chạy starter và reproduce symptom.
2. So sánh identity của các dependency mà hai component đang giữ.
3. Ghi ít nhất hai hypothesis trước khi sửa.
4. Sửa code trong `starter/` để toàn bộ application graph dùng đúng cùng một singleton instance.
5. Chạy `verify.ps1`.
6. Sau khi tự sửa mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell
- Không cần database, Docker hay cloud account.

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script thành công khi starter chứng minh được rằng state đã thay đổi ở một component nhưng component còn lại vẫn quan sát state cũ.

## Những gì cần quan sát

- `InstanceId` của `FeatureCatalog` ở update path.
- `InstanceId` của `FeatureCatalog` ở routing path.
- Giá trị route trước và sau khi update.
- Thời điểm object graph được tạo.

Không suy luận chỉ từ lifetime label `Singleton`; hãy kiểm tra object identity thực tế.

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

> Spoiler: chỉ mở sau khi đã reproduce và thử fix.

[Reference Solution](solution/README.md)

## Expected Results

### Starter

Update path và routing path in ra hai `InstanceId` khác nhau. Sau khi route được đổi thành `v2`, routing path vẫn trả `v1`.

### Sau khi sửa

Hai path dùng cùng một `InstanceId`, và routing path quan sát `v2` ngay sau update.

Nếu không reproduce được, chạy `dotnet clean` rồi chạy lại `reproduce.ps1`.

## Estimated Time

30–45 phút.
