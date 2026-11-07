# UNIT-GRAPH-001 — Cắt thông báo cảnh báo lũ làm hỏng ký tự Unicode

## Mục tiêu
Điều tra và cải tiến bộ chia thông báo cho SMS gateway có giới hạn **5 byte UTF-8 mỗi phần** trong bộ test thu nhỏ. Phân biệt độ dài chuỗi, dữ liệu gửi qua mạng và ranh giới ký tự người dùng nhìn thấy.

## Bối cảnh thực tế
Trung tâm cảnh báo lũ gửi tin nhắn có tiếng Việt, dấu kết hợp và emoji qua đối tác. Gateway từ chối một số phần tin, đôi khi màn hình của người nhận xuất hiện ký tự lỗi dù ghép các phần lại vẫn đúng. Một refactor trước đó đã vượt qua smoke test ASCII.

## Bạn cần làm gì
1. Chạy bộ kiểm tra trên mã ban đầu, ghi lại từng nhóm triệu chứng và chứng cứ.
2. Đưa ra ít nhất hai giả thuyết cạnh tranh; đối chiếu với dữ liệu từng phần.
3. Chỉnh sửa **chỉ** `starter/MessageSplitter.cs` để thỏa contract; không sửa `contract/Program.cs`.
4. Chạy verify, kiểm tra case từ chối hợp lệ và so sánh với lời giải tham khảo.

## Yêu cầu môi trường
- Windows PowerShell 5.1+ hoặc PowerShell 7; .NET SDK 8 (xem `global.json`).
- Không cần Docker, Azure, database hoặc NuGet package bên thứ ba.

## Chạy nhanh
```powershell
.\scripts\setup.ps1
.\scripts\run.ps1
.\scripts\reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chạy bộ contract trên starter gốc và **thành công khi chứng minh được ít nhất ba lỗi**. `run.ps1` in chi tiết các case để khảo sát, không phải bài kiểm tra fix.

## Những gì cần quan sát
- Mỗi phần có vượt giới hạn byte không? Có phần nào không mã hóa được bằng UTF-8 nghiêm ngặt?
- Vị trí chia có trùng ranh giới ký tự hiển thị không?
- Điều gì xảy ra khi một ký tự hiển thị đơn lẻ không thể nằm trong budget?
- Smoke test ASCII có đại diện cho payload thực tế không?

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
**Spoiler:** Chỉ xem [Reference Solution](solution/README.md) sau khi tự điều tra và thử sửa.

## Expected Results
- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)

## Estimated Time
60–75 phút (L4 / D4). Ghi chép tại [workspace](workspace/my-investigation.md).
