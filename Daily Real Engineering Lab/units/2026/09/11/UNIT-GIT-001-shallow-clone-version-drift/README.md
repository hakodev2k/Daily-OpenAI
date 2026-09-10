# UNIT-GIT-001 — Shallow Clone Version Drift

## Mục tiêu

Điều tra vì sao cùng một script tính version hoạt động đúng trên máy developer nhưng CI lại tạo artifact với version sai, dù source code và commit hiện tại giống nhau.

## Bối cảnh thực tế

Một service .NET dùng Git tag để tạo release version. Local build sinh đúng `1.2.0`, nhưng pipeline mới tối ưu checkout để giảm thời gian clone và bắt đầu sinh `0.0.0-dev`. Artifact vẫn build thành công nên lỗi chỉ được phát hiện khi deployment dashboard hiển thị version không đúng.

## Bạn cần làm gì

1. Chạy `reproduce.ps1` để tạo repository mẫu và mô phỏng checkout của CI.
2. Thu thập evidence từ Git trước khi sửa.
3. Ghi ít nhất 2 hypothesis trong `workspace/my-investigation.md`.
4. Sửa `starter/Get-Version.ps1` để version derivation hoạt động đúng trong môi trường mô phỏng.
5. Chạy `verify.ps1` và bảo đảm output là `1.2.0`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- Git 2.30+
- PowerShell 7+ hoặc Windows PowerShell
- Không cần Docker, database hay network bên ngoài

## Chạy nhanh

```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script tạo một repository local có release tag, sau đó tạo checkout mô phỏng CI và chạy logic version trong `starter/`.

## Những gì cần quan sát

- Repository nguồn có tag release hợp lệ.
- Checkout mô phỏng CI trỏ tới đúng commit mới nhất.
- Script không báo build failure nhưng trả về fallback version.
- So sánh `git log`, `git tag` và metadata repository giữa source repo và CI checkout.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ xem sau khi đã reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Before:
- source repository có tag `v1.2.0`
- CI checkout nằm đúng commit nhưng starter trả `0.0.0-dev`

After:
- cùng checkout đó trả `1.2.0`
- script vẫn fail rõ ràng nếu thực sự không có release tag nào trong repository upstream

## Estimated Time

35–50 phút.
