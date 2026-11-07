# UNIT-TEST-012 — Timeout không kết thúc công việc nền

## Mục tiêu

Điều tra một integration-test harness có timeout nhìn có vẻ hợp lý nhưng để lại công việc từ test trước tiếp tục chạy sang test sau.

## Bối cảnh thực tế

Một nhóm backend kiểm thử adapter gửi notification tới dependency chậm. Test đầu tiên phải timeout nhanh để CI không bị treo. Khi chạy riêng từng test, kết quả có vẻ ổn; khi chạy cả scenario, test kế tiếp đôi khi quan sát state không thuộc về nó.

## Bạn cần làm gì

1. Chạy starter và reproduce triệu chứng.
2. Ghi lại timeline và hypothesis trước khi sửa.
3. Chỉnh trực tiếp `starter/Program.cs`.
4. Đảm bảo timeout không để công việc của operation cũ sống vượt qua test boundary.
5. Chạy `verify.ps1`.
6. Sau đó mới so sánh với reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh

```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script chạy scenario timeout rồi bắt đầu scenario kế tiếp trên cùng test fixture. Starter phải in marker `ORPHANED_WORK_DETECTED`.

## Những gì cần quan sát

- Thời điểm timeout được báo cho caller.
- Thời điểm operation chậm thực sự kết thúc.
- State của fixture trước và sau test boundary.
- Operation nào còn có thể ghi state sau khi caller đã coi test là kết thúc.

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

⚠️ Chỉ mở sau khi đã reproduce và tự thử fix: [solution/README.md](solution/README.md)

## Expected Results

Trước khi sửa, operation của scenario timeout vẫn có thể thay đổi shared state sau test boundary. Sau khi sửa, timeout phải kết thúc theo một lifecycle có thể quan sát và cleanup; scenario kế tiếp không nhận mutation muộn từ operation trước.

## Estimated Time

35–55 phút.
