# Reference Solution

## Root cause

Access gate đọc `HttpContext.User` **trước** khi authentication middleware có cơ hội chạy. Vì vậy request có `X-User` hợp lệ vẫn bị đánh giá như anonymous và pipeline kết thúc với `401`.

Trong ASP.NET Core, middleware thực thi theo thứ tự đăng ký. Một middleware phụ thuộc vào identity phải được đặt sau middleware thiết lập identity.

## Fix

Di chuyển:

```csharp
app.UseAuthentication();
```

lên trước access gate:

```csharp
app.UseAuthentication();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/secure") &&
        !(context.User.Identity?.IsAuthenticated ?? false))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Unauthorized");
        return;
    }

    await next();
});
```

## Vì sao fix này đúng

`UseAuthentication()` gọi authentication scheme và thiết lập `HttpContext.User` trước khi middleware access gate đọc identity. Access decision sau đó được đưa ra dựa trên principal đã được xác thực.

## Verify

Chạy:

```powershell
./verify.ps1
```

Expected:

- `/public` → `200`
- `/secure` không header → `401`
- `/secure` + `X-User: ha` → `200`

## Engineering takeaway

Request pipeline ordering là dependency ordering. Nếu middleware B cần state do middleware A thiết lập, A phải chạy trước B. Khi debug lỗi authentication/authorization, đừng chỉ nhìn configuration; hãy quan sát state của `HttpContext.User` tại chính điểm access decision được thực hiện.
