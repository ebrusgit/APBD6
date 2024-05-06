using DefaultNamespace;

namespace WareHouse;

public static class Endpoints
{
    public static void RegisterEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/warehouses", async (IDbServices service) => 
            TypedResults.Ok(await service.GetEverythingWarehouse()));
        endpoints.MapGet("/api/products", async (IDbServices service) => 
            TypedResults.Ok(await service.GetEverythingProduct()));
        endpoints.MapGet("/api/order", async (IDbServices service) => 
            TypedResults.Ok(await service.GetEverythingOrder()));
        endpoints.MapGet("/api/product_warehouse", async (IDbServices service) => 
            TypedResults.Ok(await service.GetEverythingProductWarehouse()));
        endpoints.MapPost("/api/product_warehouse", async (IDbServices service, ProductWarehouseInput input) => 
            TypedResults.Ok(await service.AddProductToWarehouse(input.IdProduct, input.IdWarehouse, input.Amount, input.CreatedAt)));
    }
}