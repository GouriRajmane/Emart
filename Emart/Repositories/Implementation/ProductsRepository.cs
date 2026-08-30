using EMart.Models;
using EMart.Repositories.Interfaces;
using EMart.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace EMart.Repositories.Implementation
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public ProductsRepository(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DBCS"));
        }

        #region Get All Products

        public async Task<PagedResult<Products>> GetAll(string? searchText, int pageNumber, int pageSize)
        {
            var products = new List<Products>();
            int totalRecords = 0;

            using (SqlConnection con = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_Products", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Flag", "SELECTALL");
                    cmd.Parameters.AddWithValue("@SearchText", searchText ?? "");
                    cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", pageSize);

                    await con.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            products.Add(new Products
                            {
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                SKU = reader["SKU"].ToString(),
                                ProductName = reader["ProductName"].ToString(),
                                UnitId = Convert.ToInt32(reader["UnitId"]),
                                UnitName = reader["UnitName"].ToString(),
                                CategoryId = Convert.ToInt32(reader["CategoryId"]),
                                CategoryName = reader["CategoryName"].ToString(),
                                SubCategoryId = Convert.ToInt32(reader["SubCategoryId"]),
                                SubCategoryName = reader["SubCategoryName"].ToString(),
                                BrandId = Convert.ToInt32(reader["BrandId"]),
                                BrandName = reader["BrandName"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                Description = reader["Description"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                ThumbnailImage = reader["ThumbnailImage"].ToString()
                            });
                        }

                        if (await reader.NextResultAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                totalRecords = Convert.ToInt32(reader[0]);
                            }
                        }
                    }
                }
            }

            return new PagedResult<Products>
            {
                Items = products,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        #endregion

        #region Get Product By Id

        public async Task<ProductVM> GetById(int productId)
        {
            ProductVM model = new ProductVM();

            using (SqlConnection con = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_Products", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Flag", "SELECTBYID");
                    cmd.Parameters.AddWithValue("@ProductId", productId);

                    await con.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            model.Product = new Products
                            {


                                UnitName = reader["UnitName"].ToString(),

                                CategoryName = reader["CategoryName"].ToString(),

                                SubCategoryName = reader["SubCategoryName"].ToString(),

                                BrandName = reader["BrandName"].ToString(),
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                SKU = reader["SKU"].ToString(),
                                ProductName = reader["ProductName"].ToString(),
                                UnitId = Convert.ToInt32(reader["UnitId"]),
                                CategoryId = Convert.ToInt32(reader["CategoryId"]),
                                SubCategoryId = Convert.ToInt32(reader["SubCategoryId"]),
                                BrandId = Convert.ToInt32(reader["BrandId"]),
                                Price = Convert.ToDecimal(reader["Price"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                Description = reader["Description"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedDate"]),
                                CreatedOn = reader["CreatedOn"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreatedOn"]),
                                UpdatedOn = reader["UpdatedOn"] == DBNull.Value ? null : Convert.ToDateTime(reader["UpdatedOn"]),
                                CreatedBy = reader["CreatedBy"] == DBNull.Value ? null : Convert.ToInt32(reader["CreatedBy"]),
                                UpdatedBy = reader["UpdatedBy"] == DBNull.Value ? null : Convert.ToInt32(reader["UpdatedBy"])
                            };
                        }
                    }
                }

                // Load Product Images
                model.ProductImages = await GetImages(productId);
            }

            return model;
        }

        #endregion

        public async Task<bool> Insert(ProductVM model)
        {
            int productId = 0;

            using (SqlConnection con = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_Products", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Flag", "INSERT");
                    cmd.Parameters.AddWithValue("@SKU", model.Product.SKU);
                    cmd.Parameters.AddWithValue("@ProductName", model.Product.ProductName);
                    cmd.Parameters.AddWithValue("@UnitId", model.Product.UnitId);
                    cmd.Parameters.AddWithValue("@CategoryId", model.Product.CategoryId);
                    cmd.Parameters.AddWithValue("@SubCategoryId", model.Product.SubCategoryId);
                    cmd.Parameters.AddWithValue("@BrandId", model.Product.BrandId);
                    cmd.Parameters.AddWithValue("@Price", model.Product.Price);
                    cmd.Parameters.AddWithValue("@Quantity", model.Product.Quantity);
                    cmd.Parameters.AddWithValue("@Description", model.Product.Description ?? "");
                    cmd.Parameters.AddWithValue("@IsActive", model.Product.IsActive);
                    cmd.Parameters.AddWithValue("@CreatedBy", model.Product.CreatedBy ?? 1);

                    await con.OpenAsync();

                    object? result = await cmd.ExecuteScalarAsync();

                    if (result != null)
                    {
                        productId = Convert.ToInt32(result);
                    }
                }

                if (productId > 0 && model.Images != null && model.Images.Count > 0)
                {
                    string uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "products");

                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    foreach (IFormFile file in model.Images)
                    {
                        if (file.Length > 0)
                        {
                            string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";

                            string filePath = Path.Combine(uploadPath, fileName);

                            using (FileStream stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            using (SqlCommand imageCmd = new SqlCommand("SP_Products", con))
                            {
                                imageCmd.CommandType = CommandType.StoredProcedure;

                                imageCmd.Parameters.AddWithValue("@Flag", "INSERTIMAGE");
                                imageCmd.Parameters.AddWithValue("@ProductId", productId);
                                imageCmd.Parameters.AddWithValue("@ImagePath", "/uploads/products/" + fileName);

                                await imageCmd.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }

                con.Close();
            }

            return true;
        }

        public async Task<bool> Update(ProductVM model)
        {
            using (SqlConnection con = GetConnection())
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("SP_Products", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Flag", "UPDATE");
                    cmd.Parameters.AddWithValue("@ProductId", model.Product.ProductId);
                    cmd.Parameters.AddWithValue("@SKU", model.Product.SKU);
                    cmd.Parameters.AddWithValue("@ProductName", model.Product.ProductName);
                    cmd.Parameters.AddWithValue("@UnitId", model.Product.UnitId);
                    cmd.Parameters.AddWithValue("@CategoryId", model.Product.CategoryId);
                    cmd.Parameters.AddWithValue("@SubCategoryId", model.Product.SubCategoryId);
                    cmd.Parameters.AddWithValue("@BrandId", model.Product.BrandId);
                    cmd.Parameters.AddWithValue("@Price", model.Product.Price);
                    cmd.Parameters.AddWithValue("@Quantity", model.Product.Quantity);
                    cmd.Parameters.AddWithValue("@Description", model.Product.Description ?? "");
                    cmd.Parameters.AddWithValue("@IsActive", model.Product.IsActive);
                    cmd.Parameters.AddWithValue("@UpdatedBy", model.Product.UpdatedBy ?? 1);

                    await cmd.ExecuteNonQueryAsync();
                }

                // Upload New Images
                if (model.Images != null && model.Images.Count > 0)
                {
                    string uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "products");

                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    foreach (IFormFile file in model.Images)
                    {
                        if (file.Length > 0)
                        {
                            string fileName = Guid.NewGuid() + "_" + Path.GetFileName(file.FileName);

                            string fullPath = Path.Combine(uploadPath, fileName);

                            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            using (SqlCommand imageCmd = new SqlCommand("SP_Products", con))
                            {
                                imageCmd.CommandType = CommandType.StoredProcedure;

                                imageCmd.Parameters.AddWithValue("@Flag", "INSERTIMAGE");
                                imageCmd.Parameters.AddWithValue("@ProductId", model.Product.ProductId);
                                imageCmd.Parameters.AddWithValue("@ImagePath", "/uploads/products/" + fileName);

                                await imageCmd.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
            }

            return true;
        }

        public async Task<bool> Delete(int productId)
        {
            List<ProductImages> images = await GetImages(productId);

            foreach (var image in images)
            {
                if (!string.IsNullOrEmpty(image.ImagePath))
                {
                    string file = Path.Combine(
                        _environment.WebRootPath,
                        image.ImagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }
            }

            using (SqlConnection con = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_Products", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Flag", "DELETE");
                    cmd.Parameters.AddWithValue("@ProductId", productId);

                    await con.OpenAsync();

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return true;
        }

        public async Task<List<ProductImages>> GetImages(int productId)
        {
            List<ProductImages> images = new();

            using (SqlConnection con = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_Products", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Flag", "GETIMAGES");
                    cmd.Parameters.AddWithValue("@ProductId", productId);

                    await con.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            images.Add(new ProductImages
                            {
                                ImageId = Convert.ToInt32(reader["ImageId"]),
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                ImagePath = reader["ImagePath"].ToString()
                            });
                        }
                    }
                }
            }

            return images;
        }

        public async Task<bool> DeleteImage(int imageId)
        {
            string imagePath = string.Empty;

            using (SqlConnection con = GetConnection())
            {
                await con.OpenAsync();

                // Get Image Path
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT ImagePath FROM ProductImages WHERE ImageId=@ImageId", con))
                {
                    cmd.Parameters.AddWithValue("@ImageId", imageId);

                    object? result = await cmd.ExecuteScalarAsync();

                    if (result != null)
                    {
                        imagePath = result.ToString()!;
                    }
                }

                // Delete Database Record
                using (SqlCommand cmd = new SqlCommand("SP_Products", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Flag", "DELETEIMAGE");
                    cmd.Parameters.AddWithValue("@ImageId", imageId);

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            if (!string.IsNullOrEmpty(imagePath))
            {
                string file = Path.Combine(
                    _environment.WebRootPath,
                    imagePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }

            return true;
        }
        public async Task<List<Products>> GetLatestProducts(int count)
        {
            List<Products> products = new();

            using (SqlConnection con = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_Products", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Flag", "LATESTPRODUCTS");
                    cmd.Parameters.AddWithValue("@PageSize", count);

                    await con.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            products.Add(new Products
                            {
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                SKU = reader["SKU"].ToString(),
                                ProductName = reader["ProductName"].ToString(),
                                CategoryName = reader["CategoryName"].ToString(),
                                SubCategoryName = reader["SubCategoryName"].ToString(),
                                BrandName = reader["BrandName"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                Description = reader["Description"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                ThumbnailImage = reader["ThumbnailImage"].ToString()
                            });
                        }
                    }
                }
            }

            return products;
        }

        public async Task<List<Products>> GetFeaturedProducts(int count)
        {
            List<Products> products = new();

            using (SqlConnection con = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_Products", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Flag", "FEATUREDPRODUCTS");
                    cmd.Parameters.AddWithValue("@PageSize", count);

                    await con.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            products.Add(new Products
                            {
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                SKU = reader["SKU"].ToString(),
                                ProductName = reader["ProductName"].ToString(),
                                CategoryName = reader["CategoryName"].ToString(),
                                SubCategoryName = reader["SubCategoryName"].ToString(),
                                BrandName = reader["BrandName"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                Description = reader["Description"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                ThumbnailImage = reader["ThumbnailImage"].ToString()
                            });
                        }
                    }
                }
            }

            return products;
        }
    }
}