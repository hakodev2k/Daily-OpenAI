# Hint 03

Một hướng sửa là tách hai khái niệm:

- tạo một wrapper candidate có thể xảy ra nhiều lần
- thực thi expensive initialization bên trong wrapper chỉ một lần cho value chiến thắng

Trong .NET, `Lazy<T>` với `LazyThreadSafetyMode.ExecutionAndPublication` là một primitive phù hợp để thử nghiệm hướng này.
