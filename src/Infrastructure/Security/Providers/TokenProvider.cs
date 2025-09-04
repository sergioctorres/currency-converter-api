using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Security.Providers;

public sealed class TokenProvider(IConfiguration configuration) : ITokenProvider
{
}
