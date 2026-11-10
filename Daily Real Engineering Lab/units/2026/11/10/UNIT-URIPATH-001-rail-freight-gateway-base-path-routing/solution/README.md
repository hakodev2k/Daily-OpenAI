# Reference solution

`new Uri(baseUri, relative)` follows RFC 3986 reference resolution. A route starting with `/` is root-relative and replaces the entire path of the base URI; it does not append to `/partners/v2/`. The original code forces this root-relative form, causing 404 while preserving the host. Normalize the route contract to be relative, require a trailing slash in the base, then compose without a leading slash. See `solution/Program.cs`.

Do not concatenate raw URL strings or globally replace slash characters: query strings, escaping and authority boundaries matter. In production, log the destination path (without credentials/query secrets), validate configured base paths at startup and add integration contracts for each partner gateway. An alternative is a configured absolute endpoint per operation, trading flexibility for explicitness.

Copy `solution/Program.cs` to `starter/Program.cs`, run `./verify.ps1` and add negative cases for invalid route input.
