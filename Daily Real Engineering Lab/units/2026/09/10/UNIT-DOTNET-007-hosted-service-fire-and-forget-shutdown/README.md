# UNIT-DOTNET-007 — Background work biến mất khi host shutdown

## Mục tiêu

Điều tra một production-style failure trong .NET Generic Host khi một `BackgroundService` nhận batch công việc, bắt đầu xử lý bình thường nhưng một phần công việc không hoàn tất khi host dừng.

## Bối cảnh thực tế

Một worker xử lý ba audit-export jobs. Log cho thấy cả ba job đều đã được nhận và bắt đầu. Ngay sau đó deployment gửi shutdown signal. Process thoát sạch, không có unhandled exception, nhưng downstream chỉ nhận được một phần hoặc không nhận được kết quả nào.

Trong lab này shutdown được mô phỏng deterministic để bạn tập trung vào ownership/lifecycle thay vì timing ngẫu nhiên.

## Bạn cần làm gì

1. Chạy `./reproduce.ps1` để chứng minh symptom của starter.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Quan sát thứ tự log giữa `ExecuteAsync`, shutdown và job completion.
4. Sửa code trong `starter/` để worker không báo lifecycle hoàn tất trước phần công việc mà nó sở hữu.
5. Chạy `./verify.ps1` trên chính code bạn đã sửa.
6. Chỉ sau đó mới xem reference solution.

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

Script chỉ PASS khi starter vẫn tái hiện được symptom dự kiến.

## Những gì cần quan sát

- Có bao nhiêu job được log là `started`.
- `ExecuteAsync` kết thúc vào thời điểm nào.
- Host bắt đầu shutdown khi nào.
- Có bao nhiêu job thực sự log `completed` trước khi process kết thúc.
- Process có exception hay không.

Đừng suy luận rằng “không exception” đồng nghĩa “mọi work item đã hoàn thành”.

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

> Spoiler warning: chỉ mở sau khi đã reproduce và thử fix.

[Reference Solution](solution/README.md)

## Expected Results

**Before:** ba job được nhận, lifecycle của worker kết thúc sớm, và `completed` nhỏ hơn `3`.

**After:** cùng shutdown scenario, worker giữ ownership của work đã nhận; cả ba job hoàn tất trước khi lifecycle của worker kết thúc và output chứa `completed=3`.

## Estimated Time

35–50 phút.
