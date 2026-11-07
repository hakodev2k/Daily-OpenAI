# Reference Solution — Chỉ xem sau khi điều tra

## Symptoms
Một release artifact chứa `retired.json` và `partner-debug.json`, dù Git revision chỉ theo dõi `active.json`. Sau khi thêm `unexpected-new.json`, số file đóng gói tiếp tục tăng.

## Evidence
`git ls-files -- release/*.json` liệt kê file đã được index/tracked. `git status --short` báo các file còn sót là untracked. Manifest của starter liệt kê nhiều file hơn Git tree. Đọc `starter/Package.ps1` và đối chiếu điều kiện chọn file.

## Root cause
Quy trình đóng gói sử dụng wildcard trên filesystem của runner được tái sử dụng. Tập file tồn tại trên disk không đồng nhất với tập nội dung được phê duyệt trong commit. Một lần checkout không tự động xóa mọi untracked file. Đây là lỗi **provenance/packaging boundary**, không phải Git merge sai.

## Why this fix works
[Package.ps1 tham khảo](Package.ps1) chọn các file JSON tracked bằng `git ls-files`, kiểm tra file còn tồn tại và tạo package + manifest từ cùng một danh sách. Tập đầu vào có thể kiểm toán và không phụ thuộc file lạ trong workspace. Để thử: chép nội dung file tham khảo vào `starter/Package.ps1`, chạy `./setup.ps1` rồi `./verify.ps1`.

## Verification
- `./reproduce.ps1` trên starter gốc: PASS khi phát hiện artifact sai.
- `./verify.ps1` sau sửa: PASS khi artifact chứa đúng `active.json`, không có file lạ mới hoặc file cũ.
- Manifest phải khớp chính xác tập file đóng gói và payload phải khớp nguồn.
- Negative test phải được giữ trong CI; không chỉ kiểm tra sự hiện diện của `active.json`.

## Alternatives
1. **Ephemeral clean checkout:** dựng workspace mới mỗi build, tốt cho reproducibility; cần bảo đảm không tái dùng cache output thiếu kiểm soát.
2. **Explicit release manifest:** dùng danh sách file versioned trong repo, phù hợp nếu có generated assets; kiểm tra integrity/hash của generated outputs.
3. **`git clean -fdx` trong workspace cô lập:** có thể loại bỏ untracked/ignored file nhưng **xóa dữ liệu**, không dùng bừa trên repo thật hay workspace chung; kiểm tra đường dẫn và quyền sở hữu trước khi thực hiện.

## Wrong / tempting fixes
- Chỉ xóa `retired.json`: không ngăn một tên file lạ khác.
- Kiểm tra file `active.json` tồn tại: không phát hiện file dư.
- Chỉ chạy `git checkout` hoặc `git reset --hard`: không đảm bảo loại bỏ untracked.
- Dùng wildcard rồi lọc tên `debug`: dễ bỏ sót các file khác.
- Dùng `git clean -fdx` ở thư mục không được cô lập: có thể xóa tài liệu người dùng.

## Production implications
Đóng gói phải có source-of-truth và provenance rõ ràng; cân nhắc attestations, SHA256 manifest, immutable artifact promotion, isolated runner, file allowlist, build cache boundaries và fail-closed nếu source artifact thiếu.

## Trade-offs
`git ls-files` chỉ phù hợp khi mọi policy được release đều tracked. Nếu build có file generated hợp lệ, cần manifest/contract riêng, xác minh generated inputs và versioned build steps. Đừng mặc định loại bỏ mọi generated asset.

## What a Senior engineer should notice
Một test xác nhận đúng file cần có là chưa đủ: phải test cả **file không được phép**. Phân biệt Git commit, index, working tree và artifact; thiết kế pipeline để không phụ thuộc trạng thái tồn dư của runner.
