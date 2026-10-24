# Hint 3 — Trust boundary
ASP.NET Core có middleware xử lý `X-Forwarded-Proto` từ proxy tin cậy. Điều kiện nào để nó sửa scheme và bước đó cần xảy ra trước quyết định nào? Không nên tin `X-Forwarded-Proto` từ mọi IP.
