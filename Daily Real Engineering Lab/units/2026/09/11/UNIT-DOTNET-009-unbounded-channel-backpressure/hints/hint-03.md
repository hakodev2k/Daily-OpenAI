# Hint 03

Thử chuyển sang `Channel.CreateBounded<T>` với một capacity nhỏ, giữ `FullMode = BoundedChannelFullMode.Wait`, và tiếp tục `await` `WriteAsync` thay vì drop item.
