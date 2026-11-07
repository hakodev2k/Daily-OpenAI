# UNIT-SEC-003 — Auth Cookie mất hiệu lực sau khi đổi instance

## Mục tiêu

Điều tra vì sao protected authentication state được tạo bởi một application instance lại không đọc được ở instance khác, sau đó sửa deployment/key-management boundary mà không vô hiệu hóa cơ chế bảo vệ dữ liệu.

## Bối cảnh thực tế

Một ASP.NET Core admin portal vừa scale từ một instance lên hai instance. Login vẫn thành công, nhưng một phần người dùng bị logout ngẫu nhiên khi request tiếp theo được load balancer chuyển sang instance khác. CPU, database và session store đều bình thường.

## Bạn cần làm gì

1. Chạy starter và reproduce cross-instance failure.
2. Ghi evidence và ít nhất 3 hypotheses vào `workspace/my-investigation.md`.
3. Sửa learner-editable code trong `starter/` để hai instance có thể đọc protected payload của nhau mà vẫn giữ protection boundary.
4. Chạy `verify.ps1`.
5. Sau đó mới xem solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- instance A có tự đọc payload của mình được không
- instance B có đọc cùng payload được không
- failure xuất hiện ở encryption/decryption boundary hay business data
- thay đổi nào làm cross-instance verification pass mà không hard-code secret vào source

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

Đọc comments trong starter trước; chỉ mở solution khi cần.

## Reference Solution

> Spoiler: `solution/README.md`

## Expected Results

Before: A đọc được payload do A tạo, B không đọc được payload đó.

After: cả A và B đọc được cùng protected payload vì chúng thuộc cùng protection boundary được cấu hình có chủ đích.

## Estimated Time

40–60 phút
