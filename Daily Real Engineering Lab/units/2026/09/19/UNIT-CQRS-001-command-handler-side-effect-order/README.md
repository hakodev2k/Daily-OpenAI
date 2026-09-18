# UNIT-CQRS-001 — Command Handler Side-Effect Ordering

## Mục tiêu
Điều tra một command handler có hành vi đúng ở happy path nhưng tạo trạng thái nghiệp vụ mâu thuẫn khi persistence gặp lỗi.

## Bối cảnh thực tế
Customer Preference API cập nhật tùy chọn nhận thông báo và gửi confirmation. Support nhận ticket: một số khách hàng nhận confirmation nhưng khi tải lại trang, preference vẫn là giá trị cũ. Sự cố hiếm và thường trùng thời điểm database có transient failure.

## Bạn cần làm gì
Chạy starter, reproduce failure path, ghi evidence và hypotheses, sửa learner-editable code, chạy verify, sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell
- Không cần database hoặc message broker; lab dùng deterministic fakes.

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
- Thứ tự các event trong console timeline.
- Durable state sau khi command kết thúc.
- External notifications đã được ghi nhận hay chưa.
- Exception boundary của command.

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
[Reference Solution — spoiler](solution/README.md)

## Expected Results
Before: injected persistence failure làm command thất bại nhưng timeline cho thấy một externally visible action đã xảy ra.

After: failure path không công bố success ra ngoài; happy path vẫn persist đúng và tạo đúng một confirmation.

## Estimated Time
50 phút.