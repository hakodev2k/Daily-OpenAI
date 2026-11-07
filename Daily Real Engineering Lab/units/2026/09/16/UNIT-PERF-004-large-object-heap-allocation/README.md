# UNIT-PERF-004 — Export Endpoint Memory Pressure

## Mục tiêu
Điều tra một data-export path có functional output đúng nhưng tạo memory pressure lớn khi payload tăng kích thước.

## Bối cảnh thực tế
Một internal reporting service tạo export từ hàng nghìn records. Với dataset nhỏ, endpoint hoạt động bình thường. Khi dataset lớn hơn, process memory tăng mạnh và GC activity dày hơn dù output cuối cùng vẫn đúng.

## Bạn cần làm gì
1. Chạy starter và reproduce symptom.
2. Ghi ít nhất hai hypothesis về nguồn allocation.
3. Thu thập evidence do chương trình in ra.
4. Sửa `starter/Program.cs` để giảm peak allocation nhưng giữ nguyên output contract.
5. Chạy `verify.ps1` trên code bạn đã sửa.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script build và chạy starter với dataset cố định, sau đó kiểm tra allocation evidence.

## Những gì cần quan sát
- Kích thước output cuối cùng.
- Tổng allocated bytes được process tự đo trong operation.
- Output phải giữ nguyên sau khi tối ưu.

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
[Spoiler — chỉ xem sau khi đã tự thử](solution/README.md)

## Expected Results
Before: output đúng nhưng allocation cao hơn nhiều so với kích thước dữ liệu cuối.
After: output contract giữ nguyên và allocation giảm rõ rệt theo assertion của verify script.

## Estimated Time
35–50 phút.