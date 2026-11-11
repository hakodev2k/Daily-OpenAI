# Senior challenge
Cho phép hai luồng thay đổi `PrinterId` và `Prefix` cùng lúc với 1000 lệnh in. Đề xuất invariant: mỗi nhãn chỉ dùng một cấu hình đầy đủ cùng version. Thiết kế test có synchronization barrier, publish immutable snapshot thread-safe, và quan sát version trong log. Không dùng `Thread.Sleep` để chứng minh race.

