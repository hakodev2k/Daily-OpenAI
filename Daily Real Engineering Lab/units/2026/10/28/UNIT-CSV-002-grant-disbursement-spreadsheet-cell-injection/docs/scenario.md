# Incident brief — Grant disbursement export

**Context:** Partner Portal cho phép nhập DisplayName; dịch vụ .NET xuất BeneficiaryId (trusted, system-generated), DisplayName (untrusted), Amount (decimal).

**Synthetic timeline:** 09:00 export thành công; 09:20 kế toán báo một số tên không hiển thị nguyên văn; 09:25 xác nhận số record/cột và Amount đúng; 09:40 yêu cầu hotfix tại export boundary.

**Constraints:** Không dùng paid cloud; không chạy spreadsheet formulas; không đổi stored data hoặc numeric Amount; không coi một spreadsheet client là đại diện cho mọi client.

**Investigation:** đối chiếu CSV bytes, parsed fields, downstream cell interpretation. Tách lỗi CSV grammar khỏi semantic risks.
