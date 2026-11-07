# UNIT-KV-001 — Key Vault Secret Version Pinning

## Mục tiêu

Điều tra một sự cố configuration lifecycle: credential mới đã được rotate thành công nhưng application vẫn tiếp tục dùng credential cũ.

## Bối cảnh thực tế

Một payment integration đọc credential thông qua một secret-store abstraction mô phỏng Azure Key Vault. Operations rotate secret và xác nhận version mới đã active. Tuy nhiên một application instance đã chạy từ trước vẫn gửi credential cũ, trong khi instance khởi động mới lại hoạt động bình thường.

## Bạn cần làm gì

1. Chạy starter và reproduce sự khác biệt giữa vault state và application state.
2. Thu thập evidence từ output của application và secret store.
3. Viết ít nhất hai hypothesis trong `workspace/my-investigation.md`.
4. Sửa code/configuration trong `starter/` mà không hard-code credential.
5. Chạy `verify.ps1` để kiểm tra rotation contract.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell 7+ khuyến nghị
- Không cần Azure subscription; lab mô phỏng lifecycle local.

## Chạy nhanh

```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy scenario có hai secret versions và kiểm tra xem application có quan sát version đang active sau rotation hay không.

## Những gì cần quan sát

- Secret version nào đang active trong store.
- Application resolve secret identity như thế nào ở mỗi request.
- Restart application có thay đổi symptom hay không.
- Rotation đã thất bại, hay consumer đang giữ một reference/lifecycle không còn phù hợp?

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

Before: store báo version mới active nhưng long-running consumer vẫn quan sát credential cũ.

After: consumer giữ stable secret identity, rotation được quan sát theo lifecycle policy và functional assertions pass.

## Estimated Time

45 phút.