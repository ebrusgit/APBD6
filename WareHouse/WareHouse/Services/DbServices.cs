using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Diagnostics.Tracing;
using WareHouse;
using DefaultNamespace;

public class DbServices(IConfiguration configuration) : IDbServices
{
    private async Task<SqlConnection> GetConnection()
    {
        var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync();
        return connection;
    }
    public async Task<IEnumerable<Product_Warehouse>> GetEverythingProductWarehouse()
    {
        await using var connection = await GetConnection();
        using var command = new SqlCommand("SELECT * FROM Product_Warehouse", connection);
        using var reader = await command.ExecuteReaderAsync();
        var productWarehouses = new List<Product_Warehouse>();
        while (await reader.ReadAsync())
        {
            productWarehouses.Add(new Product_Warehouse
            {
                IdProductWarehouse = (int)reader["IdProductWarehouse"],
                IdProduct = (int)reader["IdProduct"],
                IdWarehouse = (int)reader["IdWarehouse"],
                IdOrder = (int)reader["IdOrder"],
                Amount = (int)reader["Amount"],
                Price = (decimal)reader["Price"],
                CreatedAt = (DateTime)reader["CreatedAt"]
            });
        }
        return productWarehouses;
    }
    public async Task<IEnumerable<Product>> GetEverythingProduct()
    {
        await using var connection = await GetConnection();
        using var command = new SqlCommand("SELECT * FROM Product", connection);
        using var reader = await command.ExecuteReaderAsync();
        var products = new List<Product>();
        while (await reader.ReadAsync())
        {
            products.Add(new Product
            {
                IdProduct = (int)reader["IdProduct"],
                Name = reader["Name"].ToString(),
                Description = reader["Description"].ToString(),
                Price = (decimal)reader["Price"]
            });
        }
        return products;
    }
    public async Task<IEnumerable<Warehouse>> GetEverythingWarehouse()
    {
        await using var connection = await GetConnection();
        using var command = new SqlCommand("SELECT * FROM Warehouse", connection);
        using var reader = await command.ExecuteReaderAsync();
        var warehouses = new List<Warehouse>();
        while (await reader.ReadAsync())
        {
            warehouses.Add(new Warehouse
            {
                IdWarehouse = (int)reader["IdWarehouse"],
                Name = reader["Name"].ToString()
            });
        }
        return warehouses;
    }
    public async Task<IEnumerable<Orders>> GetEverythingOrder()
    {
        await using var connection = await GetConnection();
        using var command = new SqlCommand("SELECT * FROM Orders", connection);
        using var reader = await command.ExecuteReaderAsync();
        var orders = new List<Orders>();
        var hasFullfilledColumn = reader.GetSchemaTable().Columns.Contains("FullfiledAt");
        while (await reader.ReadAsync())
        {
            var order = new Orders
            {
                IdOrder = (int)reader["IdOrder"],
                IdProduct = (int)reader["IdProduct"],
                Amount = (int)reader["Amount"],
                CreatedAt = (DateTime)reader["CreatedAt"]
            };
            if(hasFullfilledColumn&&!reader.IsDBNull(reader.GetOrdinal("FullfilledAt")))
            {
                order.FullfilledAt = (DateTime)reader["FullfilledAt"];
            }
            orders.Add(order);
        }
        return orders;
    }
    
    public async Task<Product> GetProduct(int idProduct)
    {
        await using var connection = await GetConnection();
        using var command = new SqlCommand("SELECT * FROM Product WHERE IdProduct = @idProduct", connection);
        command.Parameters.AddWithValue("idProduct", idProduct);
        using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }
        return new Product
        {
            IdProduct = (int)reader["IdProduct"],
            Name = reader["Name"].ToString(),
            Description = reader["Description"].ToString(),
            Price = (decimal)reader["Price"]
        };
    }
    public async Task<Warehouse> GetWarehouse(int idWarehouse)
    {
        await using var connection = await GetConnection();
        using var command = new SqlCommand("SELECT * FROM Warehouse WHERE IdWarehouse = @idWarehouse", connection);
        command.Parameters.AddWithValue("idWarehouse", idWarehouse);
        using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }
        return new Warehouse
        {
            IdWarehouse = (int)reader["IdWarehouse"],
            Name = reader["Name"].ToString()
        };
    }

    public async Task<Orders> getOrder(int IdProduct, int Amount, DateTime CreatedAt)
    {
        await using var connection = await GetConnection();
        using var command = new SqlCommand("SELECT * FROM Orders WHERE IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @CreatedAt ", connection);
        command.Parameters.AddWithValue("IdProduct", IdProduct);
        command.Parameters.AddWithValue("Amount", Amount);
        command.Parameters.AddWithValue("CreatedAt", CreatedAt);
        using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }
        return new Orders
        {
            IdOrder = (int)reader["IdOrder"],
            IdProduct = (int)reader["IdProduct"],
            Amount = (int)reader["Amount"],
            CreatedAt = (DateTime)reader["CreatedAt"]
        };
    }
    public async Task<Product_Warehouse> getProductWarehouse(int IdOrder)
    {
        await using var connection = await GetConnection();
        using var command = new SqlCommand("SELECT * FROM Product_Warehouse WHERE IdOrder = @IdOrder", connection);
        command.Parameters.AddWithValue("IdOrder", IdOrder);
        using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }
        return new Product_Warehouse
        {
            IdProduct = (int)reader["IdProduct"],
            IdWarehouse = (int)reader["IdWarehouse"],
            Amount = (int)reader["Amount"],
            CreatedAt = (DateTime)reader["CreatedAt"]
        };
    }
    public async Task<int> AddProductToWarehouse(int idProduct, int idWarehouse, int amount, DateTime CreatedAt)
    {
        var productWarehouseInput = new ProductWarehouseInput
        {
            IdProduct = idProduct,
            IdWarehouse = idWarehouse,
            Amount = amount,
            CreatedAt = CreatedAt
        };

        var validationContext = new ValidationContext(productWarehouseInput);
        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(productWarehouseInput, validationContext, validationResults, true);

        if (!isValid)
        {
            foreach (var validationResult in validationResults)
            {
                Console.WriteLine(validationResult.ErrorMessage);
                Console.WriteLine("Brak walidacji danych");
            }
            return -1;
        }
        await using var connection = await GetConnection();
        var product = await GetProduct(idProduct);
        var warehouse = await GetWarehouse(idWarehouse);
        if(product == null || warehouse == null)
        {
            Console.WriteLine("Product or Warehouse does not exist");
            return -1;
        }
        if(amount < 0)
        {
            Console.WriteLine("Amount cannot be negative");
            return -1;
        }
        var order = await getOrder(idProduct, amount, CreatedAt);
        if(order == null)
        {
            Console.WriteLine("Order does not exist");
            return -1;
        }
        var productWarehouse = await getProductWarehouse(order.IdOrder);
        if(productWarehouse != null)
        {
            Console.WriteLine("Product is already in the warehouse");
            return -1;
        }
        updateFullfilledAt(order.IdOrder);
        int maxId = await getMaxId()+1;
        if (maxId == -1)
        {
            return -1;
        }
        using var command = new SqlCommand(
            "INSERT INTO Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price,CreatedAt) VALUES(@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt)"
            , connection);
        command.Parameters.AddWithValue("IdWarehouse", idWarehouse);
        command.Parameters.AddWithValue("IdProduct", idProduct);
        command.Parameters.AddWithValue("IdOrder", order.IdOrder);
        command.Parameters.AddWithValue("Amount", amount);
        command.Parameters.AddWithValue("Price", product.Price*order.Amount);
        command.Parameters.AddWithValue("CreatedAt", CreatedAt);
        await command.ExecuteNonQueryAsync();
        return maxId;
    }
    private async void updateFullfilledAt(int IdOrder)
    {
        await using var connection = await GetConnection();
        using var command = new SqlCommand("UPDATE Orders SET FulfilledAt = GETDATE() WHERE IdOrder = @IdOrder", connection);
        command.Parameters.AddWithValue("IdOrder", IdOrder);
        await command.ExecuteNonQueryAsync();
    }

    private async Task<int> getMaxId()
    {
        await using var connection = await GetConnection();
        using var commandMaxId = new SqlCommand("SELECT MAX(IdProductWarehouse) FROM Product_Warehouse", connection);
        using var reader = await commandMaxId.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return -1;
        }
        if(reader.IsDBNull(0))
        {
            return 0;
        }
        return (int)reader[0];
    }
}