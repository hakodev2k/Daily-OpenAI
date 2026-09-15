# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Linux CI checkout có `starter/build.sh`, script có shebang hợp lệ và chạy được khi gọi `bash starter/build.sh`, nhưng pipeline gọi trực tiếp `./starter/build.sh` thì bị từ chối thực thi.

## 2. Evidence

`git ls-files --stage` cho thấy file được lưu với mode `100644`. Git không lưu toàn bộ POSIX permission bits, nhưng có theo dõi executable bit của regular file thông qua khác biệt giữa `100644` và `100755`.

## 3. Root cause

Executable bit chưa được persist trong Git index. Vì vậy checkout trên Linux tạo script dưới dạng regular non-executable file. Việc developer chạy `bash script.sh` chỉ yêu cầu `bash` đọc file; nó không kiểm chứng contract của direct execution.

## 4. Fix

Từ repository root:

```powershell
git update-index --chmod=+x "Daily Real Engineering Lab/units/2026/09/15/UNIT-GIT-003-ci-script-executable-bit/starter/build.sh"
```

Sau đó kiểm tra:

```powershell
git diff --cached --summary
git ls-files --stage -- "Daily Real Engineering Lab/units/2026/09/15/UNIT-GIT-003-ci-script-executable-bit/starter/build.sh"
```

Mode mong đợi là `100755`.

## 5. Why the fix works

Thay đổi được ghi vào Git index và commit, nên Linux runner nhận đúng executable metadata khi checkout thay vì phụ thuộc vào permission tình cờ trên máy developer.

## 6. How to verify

```powershell
./verify.ps1
```

Trong Linux checkout thực tế, có thể kiểm tra thêm:

```bash
./starter/build.sh
```

## 7. Alternative fixes

Pipeline có thể gọi `bash starter/build.sh`. Cách này hợp lệ nếu contract của pipeline cố ý là “script được interpreter đọc”, nhưng nó thay đổi execution contract và có thể che việc repository metadata đang sai cho các nơi khác cần direct execution.

## 8. Wrong / tempting fixes

- `chmod +x` chỉ trên workspace nhưng không stage/commit mode change: CI checkout sau vẫn lỗi.
- Thêm hoặc sửa shebang: shebang không cấp executable permission.
- Tắt step CI hoặc thêm retry: không liên quan đến nguyên nhân.
- Chuyển toàn bộ pipeline sang Windows chỉ để tránh lỗi: tăng coupling môi trường và không sửa artifact contract.

## 9. Production implications

Cross-platform CI/CD cần coi file mode là một phần của artifact/repository contract. Review diff nên chú ý mode-only changes, đặc biệt với shell scripts, hooks và entrypoint files.

## 10. Trade-offs

Persist executable bit giữ script tự mô tả đúng cách nó được dùng trên Unix-like systems. Gọi interpreter tường minh có thể phù hợp khi portability hoặc policy yêu cầu, nhưng team nên chọn một contract nhất quán thay vì dựa vào hành vi khác nhau giữa workstation và runner.

## 11. What a Senior engineer should notice

Triệu chứng xảy ra sau checkout và trước business logic, nên investigation nên bắt đầu từ artifact metadata và execution environment thay vì debug nội dung script. Một Senior engineer cũng nên kiểm tra pipeline có các script tương tự và bổ sung preflight để phát hiện mode sai trước khi deploy.
