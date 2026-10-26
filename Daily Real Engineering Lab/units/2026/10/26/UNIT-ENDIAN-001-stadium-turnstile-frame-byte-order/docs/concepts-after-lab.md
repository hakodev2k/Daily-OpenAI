# Concepts after lab

- Big-endian đặt byte có trọng số cao trước, little-endian đặt byte thấp trước.
- `BinaryPrimitives.ReadUInt16BigEndian(ReadOnlySpan<byte>)` thực thi wire contract tường minh, không phụ thuộc host CPU.
- `BitConverter` dùng native endianness, không nên dùng trực tiếp để suy luận network field.
- Dùng fixtures bất đối xứng (ví dụ `00 2A`, `03 E8`) cùng boundary values.
- Validate length, magic, version, reserved trước khi decode để tránh silent corruption.

Tham khảo API `System.Buffers.Binary.BinaryPrimitives` trên Microsoft Learn theo target framework.
