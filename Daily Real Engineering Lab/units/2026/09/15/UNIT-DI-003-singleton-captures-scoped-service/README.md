# UNIT-DI-003 — Singleton captures scoped service

## Bối cảnh
Một service xử lý catalog được đăng ký để tái sử dụng toàn ứng dụng. Sau khi bật scope validation, ứng dụng không khởi động được dù các registration nhìn có vẻ hợp lệ khi đọc riêng lẻ.

## Mục tiêu
1. Chạy starter và ghi lại exception lúc startup.
2. Xác định lifetime của từng dependency trong chuỗi `CatalogCoordinator -> RequestWorkspace`.
3. Giải thích vì sao lỗi nằm ở ownership/lifetime boundary, không phải ở constructor syntax.
4. Sửa registration/thiết kế để dependency ngắn hạn không bị giữ bởi object sống lâu hơn.
5. Chạy verification để chứng minh hai scope độc lập không dùng chung `RequestWorkspace`.

## Reproduce
```powershell
./scripts/reproduce.ps1
```

Expected starter result: host fail khi build/start với lỗi lifetime validation liên quan scoped service và singleton.

## Workspace
Ghi evidence và hypothesis vào `workspace/my-investigation.md` trước khi xem solution.

## Hints
- Hint 0: Vẽ dependency graph và ghi lifetime cạnh mỗi node.
- Hint 1: So sánh lifetime của object giữ reference với lifetime của object bị giữ.
- Hint 2: Scope validation đang phát hiện một captive dependency.
- Hint 3: Đừng chữa bằng cách tắt validation; thay đổi ownership boundary.

## Verify
Sau khi sửa `starter/Program.cs`:
```powershell
./scripts/verify.ps1
```

Verification yêu cầu host build thành công và hai scope tạo ra hai `RequestWorkspace` khác nhau.

## Reference
Chỉ mở `solution/reference-solution.md` sau khi đã thử điều tra.