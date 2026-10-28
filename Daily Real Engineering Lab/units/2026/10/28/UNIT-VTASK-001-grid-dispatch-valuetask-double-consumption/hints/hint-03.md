# Hint 03 — concept (spoiler một phần)
Một `ValueTask<T>` có thể đại diện cho nguồn chỉ được consume một lần. Khi đã có giá trị đã `await`, hãy sử dụng giá trị đó cho cả audit và receipt, thay vì consume lại cùng nguồn.
