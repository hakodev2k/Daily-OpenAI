var verify=args.Contains("--verify",StringComparer.OrdinalIgnoreCase);
var provider=new CredentialProvider(); var api=new ProtectedApi(); var client=new WorkerClient(provider,api); var failures=0;
for(var i=1;i<=6;i++){var ok=await client.SendAsync(); Console.WriteLine($"request={i} ok={ok} providerCalls={provider.CallCount}"); if(!ok) failures++; await Task.Delay(450);}
Console.WriteLine($"failures={failures}");
if(!verify){var reproduced=failures>0&&provider.CallCount==1; Console.WriteLine(reproduced?"SYMPTOM_CONFIRMED":"REPRODUCTION_NOT_OBSERVED_AS_EXPECTED"); Environment.ExitCode=reproduced?0:2; return;}
var passed=failures==0&&provider.CallCount is >=2 and <=4; Console.WriteLine(passed?"VERIFICATION_PASSED":"VERIFICATION_FAILED"); Environment.ExitCode=passed?0:1;
readonly record struct AccessToken(string Value,DateTimeOffset ExpiresOn);
sealed class CredentialProvider{public int CallCount{get;private set;} public Task<AccessToken> GetTokenAsync(){CallCount++; return Task.FromResult(new AccessToken($"token-{CallCount}",DateTimeOffset.UtcNow.AddSeconds(1.2)));}}
sealed class ProtectedApi{public Task<bool> SendAsync(AccessToken token)=>Task.FromResult(DateTimeOffset.UtcNow<token.ExpiresOn);}
sealed class WorkerClient{private readonly CredentialProvider _provider;private readonly ProtectedApi _api;private AccessToken? _cached;public WorkerClient(CredentialProvider provider,ProtectedApi api){_provider=provider;_api=api;}public async Task<bool> SendAsync(){_cached??=await _provider.GetTokenAsync();return await _api.SendAsync(_cached.Value);}}
