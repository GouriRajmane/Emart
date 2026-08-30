using EMart.Models;
using EMart.ViewModels;

namespace EMart.Repositories.Interfaces
{
    public interface IProductsRepository
    {
        Task<PagedResult<Products>> GetAll(string? searchText, int pageNumber, int pageSize);

        Task<ProductVM> GetById(int productId);

        Task<bool> Insert(ProductVM model);

        Task<bool> Update(ProductVM model);

        Task<bool> Delete(int productId);

        Task<List<ProductImages>> GetImages(int productId);

        Task<bool> DeleteImage(int imageId);

        Task<List<Products>> GetLatestProducts(int count);

        Task<List<Products>> GetFeaturedProducts(int count);
    }
}
