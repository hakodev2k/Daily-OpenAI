# UNIT-GIT-003 — CI script chạy local nhưng fail trên Linux runner

## Mục tiêu

Điều tra một lỗi CI/CD liên quan đến Git metadata và khác biệt hành vi giữa môi trường phát triển Windows và Linux runner.

## Bối cảnh thực tế

Team có một script `starter/build.sh` dùng trong pipeline. Developer có thể chạy nội dung script bằng `bash starter/build.sh`, nhưng Linux CI gọi trực tiếp `./starter/build.sh` và báo `Permission denied` ngay sau checkout.

Không thay đổi nội dung business logic của script. Hãy tìm bằng chứng trong repository để giải thích vì sao cùng một file lại có hành vi khác nhau.

## Bạn cần làm gì

1. Chạy bước reproduce.
2. Ghi lại evidence và ít nhất 2 hypotheses trong `workspace/my-investigation.md`.
3. Sửa repository metadata để Linux checkout có thể coi script là executable.
4. Chạy verify.
5. Sau khi tự giải quyết, so sánh với reference solution.

## Yêu cầu môi trường

- Git 2.x
- PowerShell 7+ hoặc Windows PowerShell 5.1
- Không cần Docker, cloud service hay package ngoài.

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/15/UNIT-GIT-003-ci-script-executable-bit"
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script sẽ kiểm tra metadata mà Git lưu cho `starter/build.sh` và mô phỏng CI preflight. Reproduction thành công khi preflight kết luận artifact checkout không đáp ứng contract executable của Linux runner.

## Những gì cần quan sát

- Mode mà Git index lưu cho `starter/build.sh`.
- Việc `bash starter/build.sh` vẫn có thể chạy không chứng minh file sẽ chạy được bằng `./starter/build.sh` sau Linux checkout.
- Nội dung file không thay đổi trong quá trình điều tra.

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

[Reference Solution — chỉ xem sau khi đã tự thử](solution/README.md)

## Expected Results

**Before:** CI preflight báo script không có executable mode trong Git index.

**After:** verify xác nhận Git index lưu executable mode cho script, trong khi nội dung script vẫn giữ nguyên.

## Estimated Time

25–40 phút.
