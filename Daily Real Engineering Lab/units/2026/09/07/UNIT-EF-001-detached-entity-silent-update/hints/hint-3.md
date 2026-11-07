# Hint 3

Bảo đảm entity tham gia persistence workflow của chính DbContext đang gọi `SaveChangesAsync`: hoặc load nó theo cách được tracking, hoặc explicit attach/update với state phù hợp.
