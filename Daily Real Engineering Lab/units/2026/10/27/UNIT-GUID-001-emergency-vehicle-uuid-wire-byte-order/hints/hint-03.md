# Hint 3
.NET 8 có overload Guid.ToByteArray(bool bigEndian) và Guid(ReadOnlySpan<byte>, bool bigEndian). Partner contract dùng UUID network byte order; encode và decode phải thống nhất.
