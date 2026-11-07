# UNIT-DIST-001 — Clock Skew và Lease Split-Brain

## Mục tiêu

Điều tra một production incident nơi hai worker cùng xử lý một partition dù hệ thống có cơ chế lease để bảo đảm single-owner.

## Bối cảnh thực tế

Hai instance của invoice settlement worker chạy ở hai node khác nhau. Bình thường mỗi partition chỉ có một owner. Sau một đợt thay đổi hạ tầng, cùng một invoice batch đôi lúc được xử lý hai lần trong vài giây, trong khi database vẫn chỉ hiển thị một lease row tại mỗi thời điểm.

## Bạn cần làm gì

1. Chạy reproduction và ghi lại timeline.
2. Dựa trên evidence, đưa ra ít nhất ba hypothesis.
3. Xác định vì sao một worker có thể takeover lease quá sớm.
4. Sửa code trong `starter/` để takeover sớm không còn xảy ra nhưng takeover hợp lệ sau expiry vẫn hoạt động.
5. Chạy `verify.ps1`.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell
- Không cần database hoặc cloud service ngoài.

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

## Những gì cần quan sát

- Worker A lấy được partition trước.
- Chỉ vài giây sau, worker B cũng được phép bắt đầu cùng partition.
- CPU, memory và queue depth không cho thấy overload.
- Lease row chuyển owner sớm hơn kỳ vọng so với timeline nghiệp vụ.

Đọc thêm evidence tại `evidence/incident-log.md` nhưng chưa xem solution.

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

> Spoiler: chỉ xem sau khi đã reproduce và thử fix.

- [Reference Solution](solution/README.md)

## Expected Results

Trước fix, reproduction phải chứng minh hai worker có thể cùng bắt đầu xử lý partition trong cùng một lease window.

Sau fix, worker B không được takeover sớm, nhưng vẫn phải takeover được khi lease thực sự hết hạn.

Nếu không reproduce được, chạy lại `dotnet run --project starter/LeaseLab.csproj`; output phải chứa `DOUBLE_PROCESSING=True` ở starter ban đầu.

## Estimated Time

45–60 phút.
