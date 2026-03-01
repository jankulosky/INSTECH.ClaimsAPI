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

public class CoversControllerTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private const string DefaultTestConnectionString = "Host=invalid-host;Database=testdb";

    [Fact]
    public async Task GetCoversReturnsConfiguredCollection()
    {
        var client = CreateClientWithCovers(
        [
            new CoverResponse
            {
                Id = "cover-1",
                StartDate = new DateTime(2026, 2, 1),
                EndDate = new DateTime(2026, 3, 1),
                Type = CoverType.Yacht,
                Premium = 1000m
            }
        ]);

        var response = await client.GetAsync("/Covers", TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        var covers = await response.Content.ReadFromJsonAsync<List<CoverResponse>>(JsonOptions, TestContext.Current.CancellationToken);
        Assert.NotNull(covers);
        Assert.Single(covers!);
    }

    [Fact]
    public async Task GetCoverByIdReturnsExpectedItem()
    {
        var client = CreateClientWithCovers(
        [
            new CoverResponse
            {
                Id = "cover-42",
                StartDate = new DateTime(2026, 2, 1),
                EndDate = new DateTime(2026, 4, 1),
                Type = CoverType.Tanker,
                Premium = 5000m
            }
        ]);

        var response = await client.GetAsync("/Covers/cover-42", TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        var cover = await response.Content.ReadFromJsonAsync<CoverResponse>(JsonOptions, TestContext.Current.CancellationToken);
        Assert.NotNull(cover);
        Assert.Equal("cover-42", cover!.Id);
        Assert.Equal(CoverType.Tanker, cover.Type);
    }

    [Fact]
    public async Task GetCoverByIdReturnsNotFoundWhenCoverDoesNotExist()
    {
        var client = CreateClientWithCovers([]);

        var response = await client.GetAsync("/Covers/missing-cover", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ComputePremiumReturnsValueFromService()
    {
        var client = CreateClientWithCovers([]);

        var request = new ComputePremiumRequest
        {
            StartDate = new DateTime(2026, 4, 1),
            EndDate = new DateTime(2026, 4, 20),
            CoverType = CoverType.Yacht
        };

        var response = await client.PostAsJsonAsync("/Covers/compute", request, TestContext.Current.CancellationToken);

        response.EnsureSuccessStatusCode();

        var premium = await response.Content.ReadFromJsonAsync<decimal>(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(12345m, premium);
    }

    private static HttpClient CreateClientWithCovers(IReadOnlyCollection<CoverResponse> covers)
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
                    services.RemoveAll<ICoverService>();
                    services.AddScoped<ICoverService>(_ => new FakeCoverService(covers));
                });
            });

        return application.CreateClient();
    }
}

