# UNIT-TEST-005 — Playwright test lúc pass lúc fail vì chờ theo thời gian cố định

Team commerce có E2E test cho thao tác publish product. Test thường pass trên laptop nhanh nhưng fail trên CI hoặc khi máy bận. API vẫn trả kết quả đúng; failure tập trung ở bước assertion trạng thái UI.

## Mục tiêu

Reproduce timing failure, phân biệt application failure với synchronization failure của test, sửa learner-editable test theo observable condition và verify.

## Yêu cầu môi trường

- Node.js 20+
- PowerShell

## Chạy nhanh

```powershell
cd "Daily Real Engineering Lab/units/2026/09/13/UNIT-TEST-005-playwright-hard-wait-flake"
npm install
npx playwright install chromium
./reproduce.ps1
```

## Cách reproduce vấn đề

Local server cố định publish latency ở 650ms. Starter test kiểm tra UI sớm hơn. `reproduce.ps1` chỉ pass khi test gốc thực sự fail.

## Những gì cần quan sát

- UI chuyển qua trạng thái trung gian trước khi hoàn tất.
- Server cuối cùng vẫn trả `Published`.
- Assertion phụ thuộc thời điểm test đọc DOM.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi ít nhất 2 hypothesis trong `workspace/my-investigation.md`.
3. Chỉ sửa `starter/publish.spec.js`; không giảm latency của server.
4. Chạy `./verify.ps1`.
5. Sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler — chỉ xem sau khi đã thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Before: Playwright test fail vì đọc `Publishing...` thay vì final state.

After: test chờ đúng observable condition và pass mà không phụ thuộc một sleep duration được đoán trước.

## Estimated Time

40 phút.
