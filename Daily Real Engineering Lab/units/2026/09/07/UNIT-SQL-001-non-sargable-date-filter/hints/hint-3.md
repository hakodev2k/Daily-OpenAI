# Hint 3

Một predicate dạng `CreatedUtc >= start AND CreatedUtc < nextDay` thường giữ được boundary semantics và cho optimizer một range trên index.
