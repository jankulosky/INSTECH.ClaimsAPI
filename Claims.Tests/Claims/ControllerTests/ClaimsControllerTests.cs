using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Claims.Contracts;
using Claims.Enums;
using Claims.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Claims.Tests;

public class ClaimsControllerTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private const string DefaultTestConnectionString = "Host=invalid-host;Database=testdb";

    [Fact]
    public async Task Get_Claims()
    {
        var client = CreateClientWithClaims(
        [
            new ClaimResponse
            {
                Id = "claim-1",
                CoverId = "cover-1",
                Name = "Port collision",
                Type = ClaimType.Collision,
                DamageCost = 1500m,
                Created = new DateTime(2026, 2, 20)
            }
        ]);

        var response = await client.GetAsync("/Claims", TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        var claims = await response.Content.ReadFromJsonAsync<List<ClaimResponse>>(JsonOptions, TestContext.Current.CancellationToken);
        Assert.NotNull(claims);
        Assert.Single(claims!);
    }

    [Fact]
    public async Task GetClaimByIdReturnsExpectedItem()
    {
        var client = CreateClientWithClaims(
        [
            new ClaimResponse
            {
                Id = "claim-42",
                CoverId = "cover-1",
                Name = "Deck fire",
                Type = ClaimType.Fire,
                DamageCost = 12000m,
                Created = new DateTime(2026, 2, 21)
            }
        ]);

        var response = await client.GetAsync("/Claims/claim-42", TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();
        var claim = await response.Content.ReadFromJsonAsync<ClaimResponse>(JsonOptions, TestContext.Current.CancellationToken);

        Assert.NotNull(claim);
        Assert.Equal("claim-42", claim!.Id);
        Assert.Equal("Deck fire", claim.Name);
    }

    [Fact]
    public async Task GetClaimByIdReturnsNotFoundWhenClaimDoesNotExist()
    {
        var client = CreateClientWithClaims([]);

        var response = await client.GetAsync("/Claims/missing-claim", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static HttpClient CreateClientWithClaims(IReadOnlyCollection<ClaimResponse> claims)
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:PrimaryDatabase"] = Environment.GetEnvironmentVariable("CLAIMS_TEST_CONNECTION_STRING")
                            ?? DefaultTestConnectionString,
                        ["Database:ApplyMigrationsOnStartup"] = "false"
                    });
                });

                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<IClaimService>();
                    services.AddScoped<IClaimService>(_ => new FakeClaimService(claims));
                });
            });

        return application.CreateClient();
    }
}

