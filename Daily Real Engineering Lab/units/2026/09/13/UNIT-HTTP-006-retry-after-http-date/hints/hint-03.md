# Hint 3

Một representation là số giây; representation còn lại là HTTP-date tuyệt đối. Delay cần được tính từ `now` và clamp về `TimeSpan.Zero` nếu timestamp đã ở quá khứ.