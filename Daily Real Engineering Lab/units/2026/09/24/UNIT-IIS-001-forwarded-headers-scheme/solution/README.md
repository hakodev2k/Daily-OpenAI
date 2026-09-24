# Reference Solution — inspect only after reproducing and attempting your own fix

## Symptoms
Public HTTPS traffic repeatedly receives HTTPS redirects after TLS terminates at the reverse proxy, while direct backend traffic behaves differently.

## Evidence
The proxy-to-backend hop is HTTP. The proxy supplies `X-Forwarded-Proto: https`, but the starter reaches scheme-dependent middleware before establishing trusted forwarded request metadata.

## Root cause
The application treats the transport scheme of the internal proxy hop as the public request scheme. `UseHttpsRedirection` therefore sees `http` and redirects to HTTPS even though the client already used HTTPS. The next proxied request repeats the same interpretation.

## Why the fix works
Configure `ForwardedHeadersOptions` for `X-Forwarded-Proto`, restrict trust to the real proxy/network in production, and call `UseForwardedHeaders()` before `UseHttpsRedirection()`. Then `Request.Scheme` represents the trusted public request boundary before redirect logic executes.

Example core change:

```csharp
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
    // Production: configure KnownProxies/KnownNetworks for your topology.
});

var app = builder.Build();
app.UseForwardedHeaders();
app.UseHttpsRedirection();
```

For this local lab, configure loopback as the trusted proxy boundary as appropriate for the installed .NET version.

## How to verify
Run `./verify.ps1`. It validates the learner-edited `starter/`, requires HTTP 200, and checks that the application observes the public scheme as `https`.

## Alternative fixes
TLS can terminate in Kestrel instead, removing this scheme translation boundary. A platform-integrated proxy configuration can also supply/normalize forwarded metadata, but the application still needs a documented trust contract.

## Wrong / Tempting Fixes
Removing HTTPS redirection hides the symptom but weakens the intended transport policy. Hard-coding `Request.Scheme = "https"` ignores non-HTTPS traffic and topology. Trusting forwarded headers from every remote client can allow spoofed scheme/host information when the app is directly reachable.

## Production implications
Document TLS termination, direct-access restrictions, trusted proxy addresses/networks, header normalization, middleware order, and health probes. Test the deployed topology rather than only Kestrel-direct local requests.

## Trade-offs
Forwarded headers preserve flexible proxy termination but add a security-sensitive trust boundary. End-to-end TLS reduces ambiguity but may increase certificate and platform operational work.

## What a Senior engineer should notice
The important abstraction is not “IIS configuration” alone; it is ownership of request identity across network hops. Scheme, host and client IP become security-relevant metadata once a proxy rewrites the transport connection.