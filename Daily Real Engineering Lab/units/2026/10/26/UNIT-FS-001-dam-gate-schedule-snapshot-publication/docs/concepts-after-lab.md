# Sau lab
Atomic visibility khác crash durability. Same-filesystem rename không tạo transaction đa file, không tự fsync và không giải quyết lost update giữa nhiều writer. Kiểm tra Windows sharing, network filesystems và failure recovery theo deployment thật.

Microsoft Learn: https://learn.microsoft.com/en-us/dotnet/api/system.io.file.move ; https://learn.microsoft.com/en-us/dotnet/api/system.io.file.replace ; https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream .
