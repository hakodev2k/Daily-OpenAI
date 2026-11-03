# Reference Solution

Partner gửi epoch milliseconds, parser lại dùng `FromUnixTimeSeconds`; giá trị vượt phạm vi và gây `ArgumentOutOfRangeException`. Sửa thành `DateTimeOffset.FromUnixTimeMilliseconds(feed.ArrivalEpochMs)` trong starter. Không dùng timezone conversion, không chia 1000 ép int và không swallow exception rồi thay `Now`. Chạy `./verify.ps1` để xác nhận timestamp và epoch zero. Trade-off: validate unit tại boundary và reject payload sai hợp đồng, không đoán unit bằng số chữ số.
