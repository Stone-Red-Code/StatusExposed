using AspNetCoreRateLimit;

using Microsoft.Extensions.Options;

namespace StatusExposed.Utilities;

public class CustomRateLimitConfiguration : RateLimitConfiguration
{
    public CustomRateLimitConfiguration(IOptions<IpRateLimitOptions> ipOptions, IOptions<ClientRateLimitOptions> clientOptions) : base(ipOptions, clientOptions) { }

    public override ICounterKeyBuilder EndpointCounterKeyBuilder => new EndpointCounterKeyBuilder();
}