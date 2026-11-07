# Hint 3
Thử để worker giữ `IServiceScopeFactory` thay vì giữ trực tiếp dependency có state, rồi tạo/dispose một scope cho mỗi processing cycle.