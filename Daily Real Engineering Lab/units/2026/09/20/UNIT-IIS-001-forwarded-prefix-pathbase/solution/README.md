# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Direct Kestrel access behaves correctly, while URLs generated behind an IIS reverse proxy omit the public `/staff` prefix and lead users to a proxy-level 404.

## 2. Evidence
The public request contains `/staff/profile`, but the application receives `/profile`. Proxy metadata carries `/staff`, while the application's effective `PathBase` remains empty when the URL is generated.

## 3. Root cause
The proxy owns a public path prefix and strips it before forwarding, but the application never restores that external prefix into its request path-base model. Link generation therefore describes the internal path rather than the public path.

## 4. Why the fix works
At the trusted proxy boundary, translate the forwarded prefix into `Request.PathBase` before routing/link generation. In this simulator, change `effectivePathBase` so it uses the trusted `ForwardedPrefix` when present. In a real ASP.NET Core deployment, implement equivalent middleware with strict trusted-proxy configuration and correct middleware ordering.

## 5. How to verify
Run `./verify.ps1`. Then test both a prefixed public request and direct/local behavior appropriate to your deployment contract. The generated public URL must be `https://portal.company.test/staff/profile`.

## 6. Alternative fixes
Configure the proxy so the application is mounted with a path-base contract it natively understands; use `UsePathBase` when the incoming path actually still contains the prefix; or configure a gateway/base URL explicitly for outbound absolute links when that better matches architecture.

## 7. Wrong or misleading fixes
Hard-coding `/staff` into every route couples application routes to one deployment. Changing route templates fixes symptoms at the wrong boundary. Trusting a client-supplied forwarded-prefix header from arbitrary sources creates a spoofing boundary; forwarded metadata must come only from trusted infrastructure.

## 8. Production implications
Path-base mistakes break redirects, OAuth callback URLs, generated links, static assets and API documentation in ways that often appear only after deployment behind a proxy. Tests should model the public-to-internal URL transformation.

## 9. Trade-offs
Proxy-owned prefixes keep applications deployment-agnostic but require an explicit forwarding contract. Application-owned prefixes simplify some link generation but couple the app to hosting topology. Explicit public-base configuration is predictable for outbound links but can become configuration drift if multiple public hosts are valid.

## 10. What a Senior engineer should notice
The important boundary is not IIS versus Kestrel; it is ownership of external request identity. Scheme, host and path base are security and correctness inputs. A robust solution documents which layer transforms each component, which headers are trusted, and where middleware restores those semantics before routing.