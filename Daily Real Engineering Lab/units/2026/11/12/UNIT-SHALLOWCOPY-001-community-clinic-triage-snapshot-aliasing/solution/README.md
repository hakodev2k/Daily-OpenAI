# Reference solution — spoiler

## Root cause
C# record `with` là shallow copy. Hai `ShiftBoard` khác identity nhưng cùng tham chiếu tới `List<Patient>`. Thay phần tử trong live list khiến snapshot đã lưu đổi theo.

## Fix
Copy list tại boundary tạo snapshot; trong ví dụ `Patient` chỉ chứa string immutable nên việc copy từng record là phòng ngừa. Xem `solution/Program.cs`.

## Verification
Sửa `starter/Program.cs`, chạy `scripts/verify.ps1`. Script chạy learner-editable starter, không chạy solution.

## Wrong fixes
- Chỉ dùng `with` trên record ngoài: vẫn chia sẻ list.
- Dùng `AsReadOnly()` trên cùng list: chặn mutation qua wrapper nhưng không chặn mutation từ live owner.
- Serialize snapshot sau khi live board thay đổi: dữ liệu đã drift, không thể phục hồi thời điểm cũ.
- Dùng lock nhưng vẫn giữ alias sau khi lock kết thúc: không giải quyết ownership.

## Trade-offs
Copy collection là O(n) memory/time. Với hàng triệu bản ghi cần cân nhắc immutable persistent collections, snapshot versioning hoặc append-only events. Bản thân `List<Patient>` vẫn mutable qua `saved.Patients`; production API nên trả read-only immutable representation và copy cả nested mutable objects nếu có.
