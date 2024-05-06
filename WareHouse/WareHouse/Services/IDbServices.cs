namespace DefaultNamespace;

public interface IDbServices
{
    Task<IEnumerable<Product_Warehouse>> GetEverythingProductWarehouse();
    Task<IEnumerable<Product>> GetEverythingProduct();
    Task<IEnumerable<Warehouse>> GetEverythingWarehouse();
    Task<IEnumerable<Orders>> GetEverythingOrder();
    Task<int> AddProductToWarehouse(int idProduct, int idWarehouse, int amount, DateTime CreatedAt);
    Task<Product> GetProduct(int idProduct);
    Task<Warehouse> GetWarehouse(int idWarehouse);
    Task<Orders> getOrder(int IdProduct, int Amount, DateTime CreatedAt);
    Task<Product_Warehouse> getProductWarehouse(int IdOrder);
}