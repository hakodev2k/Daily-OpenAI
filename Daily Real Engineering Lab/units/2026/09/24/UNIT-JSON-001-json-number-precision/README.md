# UNIT-JSON-001 — JSON Number Precision Boundary

## Mục tiêu

Điều tra một lỗi dữ liệu tại API boundary: request hợp lệ, không exception, nhưng một amount có độ chính xác cao thay đổi sau khi đi qua pipeline parse và mapping.

## Bối cảnh thực tế

Một payment-ledger importer nhận JSON từ đối tác. Các amount thông thường hoạt động đúng. Một reconciliation job phát hiện một số record có amount lưu xuống khác payload gốc ở các chữ số cuối, trong khi request vẫn trả success.

## Bạn cần làm gì

1. Chạy starter và reproduce symptom.
2. Ghi lại input, giá trị domain nhận được và delta.
3. Đưa ra ít nhất hai hypothesis trước khi sửa.
4. Sửa code trong `starter/` nhưng giữ contract JSON number và domain amount dạng `decimal`.
5. Chạy `verify.ps1`.
6. Sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7+ khuyến nghị

## Chạy nhanh

```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy starter với một payload đại diện cho reconciliation case và xác nhận rằng giá trị đi qua pipeline không còn bằng giá trị kỳ vọng.

## Những gì cần quan sát

- Payload JSON ban đầu.
- Giá trị sau deserialize/mapping.
- Kiểu dữ liệu ở từng boundary.
- Có exception hay validation failure nào xảy ra không.
- Sai lệch có xuất hiện với mọi amount hay chỉ một số shape dữ liệu.

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

⚠️ Spoiler: [Reference Solution](solution/README.md)

## Expected Results

Before: pipeline hoàn tất nhưng amount không còn chính xác theo contract. After: các regression cases giữ nguyên giá trị decimal qua JSON boundary.

## Estimated Time

45 phút.
