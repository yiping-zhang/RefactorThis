using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RefactorThis.Models;

namespace RefactorThis.Repositories
{
    public class ProductRepository: IProductRepository
    {
        public async Task<Product> RetrieveProduct(Guid id)
        {
            var conn = Helpers.NewConnection();
            await conn.OpenAsync();
            
            await using var command = conn.CreateCommand();
            command.CommandText = "select * from Products where id = $productId collate nocase";
            command.Parameters.AddWithValue("$productId", id);
            
            Product result = null;
            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                result = new Product { 
                    Id = new Guid(reader["Id"].ToString() ?? string.Empty), //TODO do we need to handle Exception thrown as a result of `new Guid(string.Empty)`??
                    Name = reader["Name"].ToString(), 
                    Description = DBNull.Value == reader["Description"] ? null : reader["Description"].ToString(),
                    Price = HandleCurrencyConversion(reader["Price"].ToString() ?? string.Empty),
                    DeliveryPrice = HandleCurrencyConversion(reader["DeliveryPrice"].ToString() ?? string.Empty)
                };
            }

            await conn.CloseAsync();
            return result;
        }

        public async Task DeleteProduct(Guid id)
        {
            var conn = Helpers.NewConnection();
            await conn.OpenAsync();

            await using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "delete from ProductOptions where productId = $productId; delete from Products where id = $productId collate nocase";
                cmd.Parameters.AddWithValue("$productId", id);
            
                await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
        }

        public async Task<Guid> CreateProduct(Product product)
        {
            var conn = Helpers.NewConnection();
            conn.Open();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText =
                "insert into Products (id, name, description, price, deliveryprice) values ($id, $name, $description, $price, $deliveryPrice)";
            var productId = Guid.NewGuid();
            cmd.Parameters.AddWithValue("$id", productId);
            cmd.Parameters.AddWithValue("$name", product.Name);
            cmd.Parameters.AddWithValue("$description", product.Description);
            cmd.Parameters.AddWithValue("$price", product.Price);
            cmd.Parameters.AddWithValue("$deliveryPrice", product.DeliveryPrice);
            
            await cmd.ExecuteNonQueryAsync();
            return productId;
        }

        public async Task<IEnumerable<Product>> RetrieveProducts(string name, int? limit, int? offset)
        {
            var conn = Helpers.NewConnection();
            await conn.OpenAsync();

            IList<Product> results = new List<Product>();
            await using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "select * from Products";
                
                if (!string.IsNullOrEmpty(name))
                {
                    cmd.CommandText += " where Name LIKE @productName";
                    cmd.Parameters.AddWithValue("@productName", name + "%");
                }
                if (limit != null)
                {
                    cmd.CommandText += " limit $limit offset $offset";
                    cmd.Parameters.AddWithValue("$limit", limit);
                    cmd.Parameters.AddWithValue("$offset", offset);
                }
                
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    results.Add(new Product
                    {
                        Id = new Guid(reader["Id"].ToString() ?? string.Empty), //TODO do we need to handle Exception thrown as a result of `new Guid(string.Empty)`??
                        Name = reader["Name"].ToString(),
                        Description = DBNull.Value == reader["Description"] ? null : reader["Description"].ToString(),
                        Price = HandleCurrencyConversion(reader["Price"].ToString() ?? string.Empty),
                        DeliveryPrice = HandleCurrencyConversion(reader["DeliveryPrice"].ToString() ?? string.Empty)
                    });
                }
            }

            await conn.CloseAsync();
            return results;
        }

        public async Task<int> UpdateProduct(Guid id, Product product)
        {
            var conn = Helpers.NewConnection();
            await conn.OpenAsync();
            int numberOfRowsAffected;
            await using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "update Products set name = $name, description = $description, price = $price, deliveryprice = $deliveryPrice where id = $id collate nocase";
                cmd.Parameters.AddWithValue("$id", id);
                cmd.Parameters.AddWithValue("$name", product.Name);
                cmd.Parameters.AddWithValue("$description", product.Description);
                cmd.Parameters.AddWithValue("$price", product.Price);
                cmd.Parameters.AddWithValue("$deliveryPrice", product.DeliveryPrice);
                numberOfRowsAffected = await cmd.ExecuteNonQueryAsync();
            }

            await conn.CloseAsync();
            return numberOfRowsAffected;
        }

        private static decimal HandleCurrencyConversion(string value)
        {
            if (value.Equals(string.Empty))
            {
                return 0;
            }

            decimal.TryParse(value, out var result);
            return result;
        }
    }
}