# Hint 03
Một UTF-8 scalar có thể chiếm nhiều byte. Tìm hiểu `System.Text.Decoder`, `flush:false` cho từng mảnh, `flush:true` khi EOF và strict fallback khi wire bị cắt dở.
