# UNIT-AUTH-001 — Token Audience Drift Between Environments

## Mục tiêu
Điều tra một ASP.NET Core API chỉ trả 401 sau khi deploy sang môi trường mới dù token vẫn được identity system phát hành bình thường.

## Bối cảnh thực tế
Internal reporting API hoạt động ở staging. Sau khi chuyển cấu hình sang production, mọi request có bearer token đều bị từ chối. Token chưa hết hạn, signature metadata vẫn khớp và application không có exception ở business handler.

## Bạn cần làm gì
1. Chạy starter và reproduce.
2. So sánh evidence của staging và production.
3. Ghi ít nhất hai hypothesis trước khi sửa.
4. Sửa learner-editable configuration/code trong `starter/`.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell
- Không cần identity provider thật; lab dùng deterministic token-claim simulator.

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
- Claims mô phỏng từ token.
- Authentication settings của từng environment.
- Validation result trước khi request tới business handler.
- Những giá trị thay đổi giữa staging và production.

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
Before: production simulation từ chối token trong khi staging chấp nhận.

After: production chấp nhận token đúng contract, đồng thời token dành cho service khác vẫn bị từ chối.

## Estimated Time
45 phút.