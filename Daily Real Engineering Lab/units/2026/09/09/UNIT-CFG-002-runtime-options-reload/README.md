# UNIT-CFG-002 — Runtime configuration thay đổi nhưng service vẫn dùng giá trị cũ

## Mục tiêu

Điều tra một lỗi configuration lifetime trong .NET: cấu hình được thay đổi thành công khi process đang chạy nhưng một service tiếp tục sử dụng giá trị trước đó.

## Bối cảnh thực tế

Một internal routing service cho phép Operations đổi downstream endpoint mà không restart process. Hệ thống báo rằng configuration source đã được cập nhật, nhưng request tiếp theo vẫn đi tới endpoint cũ. Restart ứng dụng làm vấn đề biến mất.

## Bạn cần làm gì

1. Chạy starter và reproduce symptom.
2. Ghi lại evidence trước/sau khi runtime configuration được reload.
3. Đưa ra ít nhất hai hypothesis về nơi giá trị cũ có thể bị giữ lại.
4. Sửa code trong `starter/` để request sau reload dùng endpoint mới mà không restart process.
5. Chạy `verify.ps1`.
6. Sau khi tự sửa, mới mở reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell
- Không cần database, Docker hay cloud account.

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script thành công khi nó chứng minh được symptom của starter một cách deterministic.

## Những gì cần quan sát

Tập trung vào ba mốc output:

- endpoint được dùng trước khi thay đổi configuration
- giá trị trực tiếp trong `IConfiguration` sau khi reload
- endpoint mà service thực sự dùng sau reload

Không chỉ nhìn xem configuration source có đổi hay không; hãy xác định component nào quan sát giá trị nào tại từng thời điểm.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Chạy verify.
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

Configuration source đổi sang endpoint mới, nhưng request tiếp theo vẫn sử dụng endpoint cũ.

### Sau khi sửa

Request đầu tiên dùng endpoint ban đầu; sau runtime reload, request tiếp theo dùng endpoint mới mà không restart process.

## Estimated Time

30–45 phút.
