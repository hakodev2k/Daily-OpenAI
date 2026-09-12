# UNIT-GIT-002 — File vẫn xuất hiện trong diff dù đã thêm vào .gitignore

## Mục tiêu

Điều tra sự khác biệt giữa working tree, index và ignore rules của Git trong một tình huống cấu hình local của service .NET vẫn liên tục xuất hiện trong `git status` sau khi team đã thêm file đó vào `.gitignore`.

## Bối cảnh thực tế

Một service tích hợp partner API có `appsettings.Development.json` dùng cho cấu hình local. Team đã thêm file này vào `.gitignore`, nhưng mỗi developer chỉnh endpoint local thì file vẫn xuất hiện trong diff và có nguy cơ bị commit nhầm. Một số người kết luận `.gitignore` không hoạt động.

Lab tạo một repository nhỏ hoàn toàn local, không chứa secret thật, để bạn quan sát trạng thái Git và sửa repository state đúng cách.

## Bạn cần làm gì

1. Reproduce tình huống bằng script.
2. Quan sát `git status`, trạng thái tracked/ignored và index.
3. Ghi hypothesis vào `workspace/my-investigation.md`.
4. Sửa repository trong `workspace/repo` để file cấu hình local vẫn tồn tại trên máy nhưng không còn được Git theo dõi trong các thay đổi tiếp theo.
5. Chạy `verify.ps1` hoặc `verify.sh`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- Git 2.x
- PowerShell trên Windows hoặc Bash trên Linux/macOS
- Không cần .NET SDK, Docker, Azure hay dịch vụ ngoài

## Chạy nhanh

PowerShell:

```powershell
./reproduce.ps1
```

Bash:

```bash
./reproduce.sh
```

## Cách reproduce vấn đề

Script sẽ tạo lại `workspace/repo` từ đầu, tạo lịch sử commit nhỏ, áp dụng ignore rule rồi mô phỏng developer chỉnh cấu hình local.

Sau đó script in ra bằng chứng về:

- file có đang được Git theo dõi hay không
- ignore rule có đang tác động tới file hay không
- file có còn xuất hiện trong `git status` hay không

## Những gì cần quan sát

- Output của `git status --short`.
- Kết quả kiểm tra file trong index.
- Kết quả kiểm tra ignore rule.
- Sự khác nhau giữa việc file tồn tại trong working directory và việc Git tiếp tục quản lý file đó.

README cố ý không chỉ ra chính xác command sửa lỗi.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `workspace/repo`.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Reference Solution — chỉ xem sau khi reproduce và tự thử sửa.

- [Reference Solution](solution/README.md)

## Expected Results

### Before

- Local config file tồn tại.
- Ignore rule đã có trong repository.
- Chỉnh file local vẫn tạo thay đổi mà Git báo cáo.

### After

- Local config file vẫn tồn tại trên máy.
- Ignore rule áp dụng cho file đó.
- File không còn được quản lý như một tracked path trong index.
- Những chỉnh sửa local tiếp theo không quay lại `git status`.

## Troubleshooting

Nếu script cho biết `workspace/repo` đã bị thay đổi ngoài ý muốn, chạy `reset.ps1` hoặc `reset.sh`, sau đó reproduce lại.

## Estimated Time

25–40 phút.
