# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms

Repository đã có `.gitignore` chứa `appsettings.Development.json`, nhưng chỉnh file vẫn làm `git status` báo `M appsettings.Development.json`.

## 2. Evidence

Trong starter state:

```bash
git ls-files --error-unmatch appsettings.Development.json
```

vẫn thành công, nghĩa là path đang tồn tại trong index. Đồng thời `git status --short` báo thay đổi.

## 3. Root cause

`.gitignore` chủ yếu quyết định Git có tự động coi một **untracked path** là candidate để add hay không. Nó không tự động loại một path đã được theo dõi khỏi index.

Trong lịch sử lab, file cấu hình được commit trước; ignore rule được thêm sau. Vì vậy Git tiếp tục theo dõi path đó.

## 4. Fix tham khảo

Trong `workspace/repo`:

```bash
git rm --cached appsettings.Development.json
```

Sau đó kiểm tra:

```bash
git status --short
git check-ignore appsettings.Development.json
git ls-files --error-unmatch appsettings.Development.json
```

`git rm --cached` loại path khỏi index nhưng giữ file trong working directory. `.gitignore` sau đó ngăn file local quay lại danh sách untracked candidate thông thường.

Trong repository thật, commit thay đổi index cùng `.gitignore` nếu ignore rule chưa được commit.

## 5. Why the fix works

Git không còn có index entry cho path này. Working copy vẫn tồn tại, nhưng ignore rule giờ có thể áp dụng đúng vai trò của nó đối với path không được track.

## 6. How to verify

Từ thư mục unit:

```bash
./verify.sh
```

hoặc:

```powershell
./verify.ps1
```

Verification yêu cầu đồng thời:

- file local vẫn tồn tại
- ignore rule match
- `git ls-files` không còn tìm thấy tracked path
- nội dung local vẫn được giữ nguyên

## 7. Alternative fixes

Nếu team thực sự cần commit một template, có thể giữ `appsettings.Development.example.json` hoặc một documented template trong Git, còn file local thật được ignore. Đây thường là contract rõ ràng hơn.

Nếu file phải được version-control vì chứa cấu hình chung, thì không nên untrack; thay vào đó tách phần developer-specific sang environment variables, user secrets hoặc file local khác.

## 8. Wrong / tempting fixes

### Chỉ thêm thêm nhiều pattern vào `.gitignore`

Không xử lý index entry đang tồn tại nên triệu chứng không đổi.

### Xóa file khỏi disk

Có thể làm `git status` khác đi nhưng phá mục tiêu giữ cấu hình local cho developer.

### Dùng `git update-index --assume-unchanged`

Đây không phải cơ chế chuẩn để biến một shared tracked config thành local-only file. Nó có semantics khác và dễ gây hiểu nhầm khi team thay đổi file upstream.

### Force reset mỗi lần trước commit

Chỉ che symptom và tạo quy trình dễ mất local config.

## 9. Production implications

Nếu path từng chứa **secret thật** và đã được commit, untrack file không xóa secret khỏi Git history. Khi đó cần rotate credential trước, sau đó xử lý history nếu chính sách yêu cầu.

Lab chỉ dùng endpoint giả để tránh tạo secret trong repository.

## 10. Trade-offs

Ignore local config giảm accidental diffs nhưng cũng làm team mất một nguồn cấu hình mặc định nếu không có template/documentation thay thế. Một giải pháp tốt thường đi kèm example file hoặc configuration bootstrap rõ ràng.

## 11. What a Senior engineer should notice

Vấn đề không phải “`.gitignore` bị lỗi”, mà là nhầm lẫn giữa working tree, index và ignore semantics. Senior engineer nên kiểm tra repository state bằng evidence trước khi áp dụng workaround, đồng thời phân biệt untracking file hiện tại với bài toán xử lý secret đã tồn tại trong history.
