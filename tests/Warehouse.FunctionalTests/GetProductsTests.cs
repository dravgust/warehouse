using Microsoft.VisualStudio.TestPlatform.TestHost;
using Ogooreck.API;
using Vayosoft.Core.Utilities;
using Vayosoft.Testing;
using Warehouse.Core.Application.SiteManagement.Commands;
using Warehouse.Core.Application.SiteManagement.Models;
using static Ogooreck.API.ApiSpecification;

namespace Warehouse.FunctionalTests;

public class GetProductsTests : IClassFixture<GetProductsFixture>
{
    private readonly GetProductsFixture API;

    public GetProductsTests(GetProductsFixture api) =>
        API = api;

    [Theory]
    [InlineData(0)]
    [InlineData(-20)]
    public Task NegativeOrZeroPageSize_ShouldReturn_400(int pageSize) =>
        API.Given(URI($"/api/products/?page=1&size={pageSize}"))
            .When(GET)
            .Then(BAD_REQUEST);
}

public class GetProductsFixture : ApiSpecification<Program>, IAsyncLifetime
{
    public List<ProductDto> RegisteredProducts { get; } = new();

    public GetProductsFixture() : base(new TestWebApplicationFactory<Program>()) { }

    public async Task InitializeAsync()
    {
        var productsToRegister = new[]
        {
            new SetProduct
            {
                Id = GuidGenerator.New().ToString(),
                Name = "ValidName",
                Description = "ValidDescription",
            },
            new SetProduct
            {
                Id = GuidGenerator.New().ToString(),
                Name = "OtherValidName",
                Description = "OtherValidName",
            },
            new SetProduct
            {
                Id = GuidGenerator.New().ToString(),
                Name = "AnotherValidName",
                Description = "AnotherValidName",
            },
        };

        foreach (var registerProduct in productsToRegister)
        {
            var registerResponse = await Send(
                new ApiRequest(POST, URI("/api/products"), BODY(registerProduct))
            );

            await CREATED(registerResponse);

            var createdId = registerResponse.GetCreatedId<Guid>();

            RegisteredProducts.Add(new ProductDto
            {
                Id = createdId.ToString(),
                Name = registerProduct.Name,
                Description = registerProduct.Description,
                Metadata = registerProduct.Metadata
            });
        }
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
