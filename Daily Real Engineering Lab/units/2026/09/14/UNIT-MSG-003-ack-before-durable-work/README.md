# UNIT-MSG-003 — Message Disappears After Consumer Crash

## Mục tiêu

Điều tra một failure boundary trong message consumer khiến message không còn được xử lý lại sau khi consumer dừng giữa chừng.

## Bối cảnh thực tế

Một background worker nhận message yêu cầu tạo invoice projection. Trong happy path hệ thống hoạt động bình thường. Tuy nhiên, nếu process dừng đúng một thời điểm trong lúc xử lý, message không còn xuất hiện để retry nhưng projection cần thiết cũng chưa tồn tại.

## Bạn cần làm gì

- Reproduce trạng thái mất công việc bằng starter.
- Ghi hypothesis về lifecycle của message và durable business work.
- Sửa `starter/` để một lần dừng giữa chừng không làm mất message.
- Giữ happy path đúng và tránh tạo thêm side effect ngoài yêu cầu.
- Chạy `verify.ps1` để kiểm tra learner-editable code.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/14/UNIT-MSG-003-ack-before-durable-work"
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Starter dùng local deterministic queue simulator; không cần Azure Service Bus hay broker bên ngoài.

## Những gì cần quan sát

- Happy path tạo projection thành công.
- Khi consumer bị dừng ở failure point mô phỏng, projection chưa được tạo.
- Lần chạy kế tiếp không còn work item để hoàn tất operation ban đầu.

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

> Spoiler: chỉ xem sau khi đã reproduce và tự thử fix.

[Reference Solution](solution/README.md)

## Expected Results

**Before**

- Reproduction xác nhận một crash window có thể để lại `projection=false` và message không còn available.

**After**

- Happy path vẫn thành công.
- Sau failure mô phỏng, work item vẫn có thể được xử lý ở lần chạy tiếp theo.
- `verify.ps1` pass trên code trong `starter/` sau khi bạn sửa.

## Estimated Time

25–40 phút.
