using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RefactorThis.Models;

namespace RefactorThis.Repositories
{
    public class ProductOptionRepository: IProductOptionRepository
    {
        public async Task<IEnumerable<ProductOption>> RetrieveOptions(Guid productId)
        {
            var conn = Helpers.NewConnection();
            await conn.OpenAsync();

            IList<ProductOption> results = new List<ProductOption>();
            await using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "select * from ProductOptions where productId = $productId;";
                cmd.Parameters.AddWithValue("$productId", productId);

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    results.Add(new ProductOption()
                    {
                        Id = new Guid(reader["Id"].ToString() ?? string.Empty), //TODO do we need to handle Exception thrown as a result of `new Guid(string.Empty)`??
                        Name = reader["Name"].ToString(),
                        Description = DBNull.Value == reader["Description"] ? null : reader["Description"].ToString(),
                    });
                }
            }

            await conn.CloseAsync();
            return results;
        }

        public async Task<Guid> AddOption(Guid productId, ProductOption option)
        {
            var conn = Helpers.NewConnection();
            conn.Open();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText =
                "insert into ProductOptions (id, name, description, productId) values ($id, $name, $description, $productId)";
            var optionId = Guid.NewGuid();
            cmd.Parameters.AddWithValue("$id", optionId);
            cmd.Parameters.AddWithValue("$name", option.Name);
            cmd.Parameters.AddWithValue("$description", option.Description);
            cmd.Parameters.AddWithValue("$productId", productId);

            await cmd.ExecuteNonQueryAsync();
            await conn.CloseAsync();
            return optionId;
        }

        public async Task<ProductOption> GetOption(Guid optionId)
        {
            var conn = Helpers.NewConnection();
            await conn.OpenAsync();

            await using var command = conn.CreateCommand();
            command.CommandText = "select * from ProductOptions where id = $id collate nocase";
            command.Parameters.AddWithValue("$id", optionId);

            ProductOption result = null;
            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                result = new ProductOption {
                    Id = new Guid(reader["Id"].ToString() ?? string.Empty),
                    Name = reader["Name"].ToString(),
                    Description = DBNull.Value == reader["Description"] ? null : reader["Description"].ToString()
                };
            }

            await conn.CloseAsync();
            return result;
        }

        public async Task<int> UpdateOption(Guid productId, Guid optionId, ProductOption option)
        {
            var conn = Helpers.NewConnection();
            await conn.OpenAsync();
            int numberOfRowsAffected;
            await using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "update ProductOptions set name = $name, description = $description where id = $optionId and productId = $productId collate nocase";
                cmd.Parameters.AddWithValue("$optionId", optionId);
                cmd.Parameters.AddWithValue("$productId", productId);
                cmd.Parameters.AddWithValue("$name", option.Name);
                cmd.Parameters.AddWithValue("$description", option.Description);
                numberOfRowsAffected = await cmd.ExecuteNonQueryAsync();
            }

            await conn.CloseAsync();
            return numberOfRowsAffected;
        }

        public async Task DeleteOption(Guid productId, Guid optionId)
        {
            var conn = Helpers.NewConnection();
            await conn.OpenAsync();

            await using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "delete from ProductOptions where productId = $productId and id = $optionId collate nocase";
                cmd.Parameters.AddWithValue("$productId", productId);
                cmd.Parameters.AddWithValue("$optionId", optionId);

                await cmd.ExecuteNonQueryAsync();
            }
            await conn.CloseAsync();
        }
    }
}
